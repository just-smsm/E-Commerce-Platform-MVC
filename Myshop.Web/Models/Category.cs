namespace Myshop.Web.Models
{
    public class Category : ModelBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;
        public ICollection<Product> Products { get; set; }
    }
}
