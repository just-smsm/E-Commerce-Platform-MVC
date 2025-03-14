using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Myshop.Web.Models;
using Myshop.Web.Service;
using Myshop.Web.UnitOfWorks;
using System.Threading.Tasks;

namespace Myshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public DashboardController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var categorycount = await _unitOfWork.Category.GetAllAsync();
            var productCount = await _unitOfWork.Product.GetAllAsync();
            var orderCount = await _unitOfWork.OrderHeader.GetAllAsync();
            var approvedOrdersCount = await _unitOfWork.OrderHeader.GetAllAsync(x => x.OrderStatus == SD.Approve);
            var proccessingOrdersCount = await _unitOfWork.OrderHeader.GetAllAsync(x => x.OrderStatus == SD.Proccessing);
            var shippingOrdersCount = await _unitOfWork.OrderHeader.GetAllAsync(x => x.OrderStatus == SD.Shipped);
            ViewBag.ProductCount = productCount?.Count() ?? 0;
            ViewBag.OrderCount = orderCount?.Count() ?? 0;
            ViewBag.UsersCount = _userManager.Users?.Count() ?? 0; // Assuming you have a method to get users
            ViewBag.ApprovedOrdersCount = approvedOrdersCount?.Count() ?? 0;
            ViewBag.ProccessingOrdersCount = proccessingOrdersCount?.Count() ?? 0;
            ViewBag.ShippingOrdersCount = shippingOrdersCount?.Count() ?? 0;
            ViewBag.categorycount = categorycount?.Count() ?? 0;
            return View();
        }
    }
}
