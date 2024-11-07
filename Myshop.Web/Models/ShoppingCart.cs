using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using NuGet.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Myshop.Web.Models
{
    public class ShoppingCart:ModelBase
    {
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }
        [Range(1,100,ErrorMessage ="you must enter value between 1 to 100")]
        public int Count { get; set; }
        public string AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        [ValidateNever]
        public AppUser Appuser { get; set; }

    }
}
