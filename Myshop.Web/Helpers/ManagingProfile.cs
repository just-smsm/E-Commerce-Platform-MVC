using AutoMapper;
using Myshop.Web.Models;
using Myshop.Web.ViewModels;

namespace Myshop.Web.Helpers
{
    public class ManagingProfile:Profile
    {
        public ManagingProfile()
        {
            CreateMap<Category, CategoryViewModel>().ReverseMap();
            CreateMap<Product,ProductViewModel>()
                //  .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Category.Name))
                .ReverseMap();
            CreateMap<ShoppingCart,ShoppingCartViewComponent>().ReverseMap();

        }
    }
}
