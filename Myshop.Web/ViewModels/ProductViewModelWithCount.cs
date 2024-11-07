namespace Myshop.Web.ViewModels
{
    public class ProductViewModelWithCount
    {
        public int Id { get; set; }
        public ProductViewModel product { get; set; }
        public int count { get; set; }
    }
}
