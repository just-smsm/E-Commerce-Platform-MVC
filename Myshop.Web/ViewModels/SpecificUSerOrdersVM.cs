namespace Myshop.Web.ViewModels
{
    public class SpecificUSerOrdersVM
    {
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public List<SpecificUSerOrdersProductsVM> OrderItems { get; set; }
        
    }
}
