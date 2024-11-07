using Myshop.Web.Models;

namespace Myshop.Web.ViewModels
{
    public class OrderVM
    {
        public int Id { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }

}
