using Myshop.Web.Models;

namespace Myshop.Web.ViewModels
{
    public class ShoppinCartViewModel
    {
        public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
