namespace Myshop.Web.Models
{
    public class Product : ModelBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PictureUrl { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
