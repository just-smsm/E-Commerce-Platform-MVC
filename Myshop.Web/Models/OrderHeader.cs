using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Myshop.Web.Models
{
    public class OrderHeader:ModelBase
    {
        public string AppUserId { get; set; }
        [ValidateNever]
        public AppUser AppUser { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string ? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public DateTime PaymentDate { get; set; }
        public string ?SessionId { get; set; }
        public string ?PaymentIntentId { get; set; }
        public string? DisplayName { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }
}
