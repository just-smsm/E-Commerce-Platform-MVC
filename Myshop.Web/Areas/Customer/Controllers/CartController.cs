using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Myshop.Web.IRepository;
using Myshop.Web.Models;
using Myshop.Web.Service;
using Myshop.Web.UnitOfWorks;
using Myshop.Web.ViewModels;
using Stripe.Checkout;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Myshop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public CartController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet("GetCartCount")]
        public IActionResult GetCartCount()
        {
            var cartCount = _httpContextAccessor.HttpContext.Session.GetInt32(SD.SessionKey) ?? 0;
            return Ok(new { count = cartCount });
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var shoppingcartvm = new ShoppinCartViewModel
            {
                ShoppingCarts = await _unitOfWork.ShoppingCart.GetAllAsync(
                    u => u.AppUserId == claim.Value,
                    includeProperties: "Product")
            };

            foreach (var item in shoppingcartvm.ShoppingCarts)
            {
                shoppingcartvm.TotalPrice += (item.Count * item.Product.Price);
            }

            return View(shoppingcartvm);
        }

        [HttpGet]
        [ActionName("Summary")]
        public async Task<IActionResult> Summary()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = claim.Value;

            var shoppingCarts = await _unitOfWork.ShoppingCart.GetAllAsync(
                u => u.AppUserId == userId,
                includeProperties: "Product");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var shoppingCartVM = new ShoppinCartViewModel
            {
                ShoppingCarts = shoppingCarts,
                OrderHeader = new OrderHeader
                {
                    DisplayName = user.DisplayName,
                    Address = user.Address,
                    City = user.City,
                    Phone = user.PhoneNumber
                }
            };

            foreach (var item in shoppingCartVM.ShoppingCarts)
            {
                shoppingCartVM.TotalPrice += (item.Count * item.Product.Price);
            }

            return View(shoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ShoppinCartViewModel shoppingCartVM)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = claim.Value;
            shoppingCartVM.ShoppingCarts = await _unitOfWork.ShoppingCart.GetAllAsync(
                u => u.AppUserId == userId,
                includeProperties: "Product");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            shoppingCartVM.TotalPrice = 0; // Reset total price before calculation
            foreach (var item in shoppingCartVM.ShoppingCarts)
            {
                shoppingCartVM.TotalPrice += (item.Count * item.Product.Price);
            }

            shoppingCartVM.OrderHeader = new OrderHeader
            {
                OrderStatus = SD.Pending,
                PaymentStatus = SD.Pending,
                OrderDate = DateTime.Now,
                AppUserId = userId,
                Address = user.Address,
                City = user.City,
                DisplayName = user.DisplayName,
                Phone = user.Phone,
                TotalPrice = shoppingCartVM.TotalPrice,
            };

            await _unitOfWork.OrderHeader.CreateAsync(shoppingCartVM.OrderHeader);
            await _unitOfWork.CompleteAsync();

            foreach (var item in shoppingCartVM.ShoppingCarts)
            {
                // Check if the product already exists in OrderDetails
                var existingOrderDetail = await _unitOfWork.OrderDetail.GetFirstOrDefaultAsync(
                    od => od.ProductId == item.ProductId && od.OrderHeaderId == shoppingCartVM.OrderHeader.Id);

                if (existingOrderDetail != null)
                {
                    // If it exists, increase the count
                    existingOrderDetail.Count += item.Count;
                    await _unitOfWork.OrderDetail.UpdateAsync(existingOrderDetail);
                }
                else
                {
                    // If it doesn't exist, create a new OrderDetail
                    var orderDetail = new OrderDetail
                    {
                        ProductId = item.ProductId,
                        Count = item.Count,
                        Price = item.Product.Price,
                        OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                    };
                    await _unitOfWork.OrderDetail.CreateAsync(orderDetail);
                }
            }

            await _unitOfWork.CompleteAsync();
            HttpContext.Session.SetInt32(SD.SessionKey, 0);

            var domain = "https://localhost:7102/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + "Customer/Cart/Index"
            };

            foreach (var item in shoppingCartVM.ShoppingCarts)
            {
                var sessionLineItemOptions = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                    },
                    Quantity = item.Count
                };

                options.LineItems.Add(sessionLineItemOptions);
            }

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            shoppingCartVM.OrderHeader.PaymentIntentId = session.PaymentIntentId;
            shoppingCartVM.OrderHeader.SessionId = session.Id;
            await _unitOfWork.CompleteAsync();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            OrderHeader header = await _unitOfWork.OrderHeader.GetByIdAsync(id);
            if (header == null)
            {
                return NotFound();
            }

            var service = new SessionService();
            Session session = service.Get(header.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                await _unitOfWork.OrderHeader.UpdateOrderStatus(id, SD.Approve, SD.Approve);
                header.PaymentIntentId = session.PaymentIntentId;
                await _unitOfWork.CompleteAsync();
            }

            List<ShoppingCart> shoppingCarts = (await _unitOfWork.ShoppingCart.GetAllAsync(u => u.AppUserId == header.AppUserId)).ToList();

            // Uncomment if you want to delete the shopping carts after order is confirmed
            await _unitOfWork.ShoppingCart.DeleteRange(shoppingCarts);
            await _unitOfWork.CompleteAsync();

            return View("OrderConfirmation", header);
        }
    }
}
