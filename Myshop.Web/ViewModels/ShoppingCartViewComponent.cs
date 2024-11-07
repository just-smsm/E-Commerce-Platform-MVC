
using Microsoft.AspNetCore.Mvc;
using Myshop.Web.Service;
using Myshop.Web.UnitOfWorks;
using System.Security.Claims;

namespace Myshop.Web.ViewModels
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var sessionCount = HttpContext.Session.GetInt32(SD.SessionKey);
                if (sessionCount != null)
                {
                    return View(sessionCount);
                }
                else
                {
                    var cartItemsCount = await _unitOfWork.ShoppingCart
                        .GetAllAsync(x => x.AppUserId == claim.Value);

                    var count = cartItemsCount.Count();
                    HttpContext.Session.SetInt32(SD.SessionKey, count);

                    return View(count);
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
