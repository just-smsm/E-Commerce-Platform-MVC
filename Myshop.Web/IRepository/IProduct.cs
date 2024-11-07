using Myshop.Web.Models;

namespace Myshop.Web.IRepository
{
    public interface IProduct : IGenericRepository<Product>
    {
        public Task<IEnumerable<Product>> GetProductsWithCategoryAsync();
    }
}
