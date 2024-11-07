using System.ComponentModel.DataAnnotations;

namespace Myshop.Web.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        public string PictureUrl { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }
    }
}
