using Microsoft.AspNetCore.Mvc;
using Myshop.Web.Models;

namespace Myshop.Web.IRepository
{
    public interface ICartService
    {
        public Task<IActionResult> AddToCartAsync(ShoppingCart shoppingCart, string userId);
    }

}
