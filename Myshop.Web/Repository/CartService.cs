using Microsoft.AspNetCore.Mvc;
using Myshop.Web.IRepository;
using Myshop.Web.Models;
using Myshop.Web.Service;
using Myshop.Web.UnitOfWorks;
using Microsoft.AspNetCore.Http;
using Myshop.Web.Data;
namespace Myshop.Web.Repository
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor; // ✅ Add this

        public CartService(ApplicationDbContext context, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor; // ✅ Assign it
        }

        public async Task<IActionResult> AddToCartAsync(ShoppingCart shoppingCart, string userId)
        {
            var cartObj = await _unitOfWork.ShoppingCart.GetFirstOrDefaultAsync(userId, shoppingCart.ProductId);
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

            var cartItems = await _unitOfWork.ShoppingCart.GetAllAsync(x => x.AppUserId == userId);

            // ✅ Use _httpContextAccessor.HttpContext instead of HttpContext directly
            _httpContextAccessor.HttpContext.Session.SetInt32(SD.SessionKey, cartItems.Count());

            return new JsonResult(new { success = true, message = "Product added to cart" });
        }
    }

}
