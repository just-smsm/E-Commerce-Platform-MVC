using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Myshop.Web.Data;
using Myshop.Web.Models;
using Myshop.Web.Service;
using Myshop.Web.UnitOfWorks;
using Myshop.Web.ViewModels;
using System.Security.Claims;
using X.PagedList;

namespace Myshop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CustomerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public CustomerController(IUnitOfWork unitOfWork, IMapper mapper, ApplicationDbContext dbContext)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1; // Default to the first page
            const int pageSize = 8; // Number of products per page

            var products = await _unitOfWork.Product.GetAllAsync();
            var productViewModels = _mapper.Map<IEnumerable<ProductViewModel>>(products);
            var pagedProducts = productViewModels.ToPagedList(pageNumber, pageSize);

            return View(pagedProducts);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int productId)
        {
            var product = await _unitOfWork.Product.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound(); // Product not found
            }

            var shoppingCart = new ShoppingCart
            {
                Product = product,
                Count = 1
            };

            return View(shoppingCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return Unauthorized(); // User not authenticated
            }

            shoppingCart.AppUserId = claim.Value;
            ModelState.Remove(nameof(shoppingCart.AppUserId));

            if (!ModelState.IsValid)
            {
                return View(shoppingCart); // Return the view with validation errors
            }

            var cartObj = await _unitOfWork.ShoppingCart.GetFirstOrDefaultAsync(claim.Value, shoppingCart.ProductId);
            if (cartObj == null)
            {
                await _unitOfWork.ShoppingCart.CreateAsync(shoppingCart);
            }
            else
            {
                cartObj.Count += shoppingCart.Count;
                await _unitOfWork.ShoppingCart.UpdateAsync(cartObj);
            }

            await _unitOfWork.CompleteAsync();

            var cartItems = await _unitOfWork.ShoppingCart.GetAllAsync(x => x.AppUserId == claim.Value);
            HttpContext.Session.SetInt32(SD.SessionKey, cartItems.Count());

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateQuantity(int cartId, int count)
        {
            var cart = await _unitOfWork.ShoppingCart.GetByIdAsync(cartId);
            if (cart == null)
            {
                return NotFound();
            }

            cart.Count += count;
            if (cart.Count <= 0)
            {
                await _unitOfWork.ShoppingCart.DeleteAsync(cart.Id);
            }
            else
            {
                await _unitOfWork.ShoppingCart.UpdateAsync(cart);
            }

            await _unitOfWork.CompleteAsync();

            var cartItems = await _unitOfWork.ShoppingCart.GetAllAsync(x => x.AppUserId == cart.AppUserId);
            HttpContext.Session.SetInt32(SD.SessionKey, cartItems.Count());

            return RedirectToAction("Index", "Cart");
        }
        [HttpGet]
        public async Task<IActionResult> SpecificUSerOrders()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return Unauthorized();
            }

            string userId = claim.Value;
            var orders = await _unitOfWork.OrderHeader.GetAllAsync(u => u.AppUserId == userId, includeProperties: "AppUser");
            return View(orders);
        }
        public async Task<IActionResult> DeleteProduct(int cartId)
        {
            var cart = await _unitOfWork.ShoppingCart.GetByIdAsync(cartId);
            if (cart != null)
            {
                await _unitOfWork.ShoppingCart.DeleteAsync(cartId);
                await _unitOfWork.CompleteAsync();

                var cartItems = await _unitOfWork.ShoppingCart.GetAllAsync(x => x.AppUserId == cart.AppUserId);
                HttpContext.Session.SetInt32(SD.SessionKey, cartItems.Count());
            }

            return RedirectToAction("Index", "Cart");
        }
    }
}
