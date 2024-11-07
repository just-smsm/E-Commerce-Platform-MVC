using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Myshop.Web.Models
{
    public class OrderDetail:ModelBase
    {
        public int OrderHeaderId { get; set; }
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }
        public int ProductId { get; set; }
        [ValidateNever]
        public Product Product { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }

    }
}
