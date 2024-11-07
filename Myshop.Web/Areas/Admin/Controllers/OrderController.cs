using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Myshop.Web.Service;
using Myshop.Web.UnitOfWorks;
using Myshop.Web.ViewModels;
using Stripe;
using Stripe.Climate;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Myshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _unitOfWork.OrderHeader.GetAllAsync(includeProperties: "AppUser");
            return View(orders);
        }
        public async Task<IActionResult> ApprovedOrders()
        {
            var orders = await _unitOfWork.OrderHeader.GetAllAsync(u=>u.OrderStatus==SD.Approve,includeProperties: "AppUser");
            return View(orders);
        }
        public async Task<IActionResult> ProccessingOrders()
        {
            var orders = await _unitOfWork.OrderHeader.GetAllAsync(u => u.OrderStatus == SD.Proccessing, includeProperties: "AppUser");
            return View(orders);
        }
        public async Task<IActionResult> ShippingOrders()
        {
            var orders = await _unitOfWork.OrderHeader.GetAllAsync(u => u.OrderStatus == SD.Shipped, includeProperties: "AppUser");
            return View(orders);
        }
        
        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var orderHeader = await _unitOfWork.OrderHeader.GetOrderWithAppUser(id);
            if (orderHeader == null)
            {
                return NotFound();
            }

            var orderDetails = await _unitOfWork.OrderDetail.GetOrderWithAppUser(id); // Filter by order ID

            var orderVM = new OrderVM
            {
                OrderHeader = orderHeader,
                OrderDetails = orderDetails.ToList()
            };

            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderDetails(OrderVM orderVM)
        {
           

            var order = await _unitOfWork.OrderHeader.GetByIdAsync(orderVM.OrderHeader.Id);
            if (order == null)
            {
                TempData["toastrMessage"] = "Order not found.";
                TempData["toastrType"] = "error";
                return RedirectToAction("Index", "Order");
            }

            order.DisplayName = orderVM.OrderHeader.DisplayName;
            order.City = orderVM.OrderHeader.City;
            order.Address = orderVM.OrderHeader.Address;
            order.Phone = orderVM.OrderHeader.Phone;
            await _unitOfWork.CompleteAsync();
            if (!string.IsNullOrWhiteSpace(orderVM.OrderHeader.Carrier))
            {
                order.Carrier = orderVM.OrderHeader.Carrier;
            }

            if (!string.IsNullOrWhiteSpace(orderVM.OrderHeader.TrackingNumber))
            {
                order.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            }
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.OrderHeader.UpdateAsync(order);
            await _unitOfWork.CompleteAsync();

            TempData["toastrMessage"] = "Data has been updated successfully";
            TempData["toastrType"] = "updated";

            return RedirectToAction("OrderDetails", new { id = order.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartProcess()
        {
            var order = await _unitOfWork.OrderHeader.GetByIdAsync(OrderVM.OrderHeader.Id);
            if (order == null)
            {
                TempData["toastrMessage"] = "Order not found.";
                TempData["toastrType"] = "error";
                return RedirectToAction("Index", "Order");
            }

            order.OrderStatus = SD.Proccessing;
            if (!string.IsNullOrWhiteSpace(OrderVM.OrderHeader.Carrier))
            {
                order.Carrier = OrderVM.OrderHeader.Carrier;
            }

            if (!string.IsNullOrWhiteSpace(OrderVM.OrderHeader.TrackingNumber))
            {
                order.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

            await _unitOfWork.OrderHeader.UpdateAsync(order);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("OrderDetails", new { id = order.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartShip()
        {
            var order = await _unitOfWork.OrderHeader.GetByIdAsync(OrderVM.OrderHeader.Id);
            if (order == null)
            {
                TempData["toastrMessage"] = "Order not found.";
                TempData["toastrType"] = "error";
                return RedirectToAction("Index", "Order");
            }

            order.OrderStatus = SD.Shipped;
            order.ShippingDate = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(OrderVM.OrderHeader.TrackingNumber))
            {
                order.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

            if (!string.IsNullOrWhiteSpace(OrderVM.OrderHeader.Carrier))
            {
                order.Carrier = OrderVM.OrderHeader.Carrier;
            }

            await _unitOfWork.OrderHeader.UpdateAsync(order);
            await _unitOfWork.CompleteAsync();

            TempData["toastrMessage"] = "Order has been shipped successfully";
            TempData["toastrType"] = "updated";

            return RedirectToAction("OrderDetails", new { id = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder()
        {
            var order = await _unitOfWork.OrderHeader.GetByIdAsync(OrderVM.OrderHeader.Id);
            if (order == null)
            {
                TempData["toastrMessage"] = "Order not found.";
                TempData["toastrType"] = "error";
                return RedirectToAction("Index", "Order");
            }

            if (order.PaymentStatus == SD.Approve)
            {
                var option = new RefundCreateOptions
                {
                    Reason=RefundReasons.RequestedByCustomer,
                    PaymentIntent=order.PaymentIntentId,
                };
                var service = new RefundService();
                Refund refund = service.Create(option);
                await _unitOfWork.OrderHeader.UpdateOrderStatus(order.Id, SD.Cancelled,SD.Refund);
            }
            else
            {
                order.OrderStatus = SD.Cancelled;
                order.PaymentStatus = SD.Cancelled;
            }

            await _unitOfWork.OrderHeader.UpdateAsync(order);
            await _unitOfWork.CompleteAsync();

            TempData["toastrMessage"] = "Order has been canceled successfully";
            TempData["toastrType"] = "updated";

            return RedirectToAction("OrderDetails", new { id = OrderVM.OrderHeader.Id });
        }

    }
}