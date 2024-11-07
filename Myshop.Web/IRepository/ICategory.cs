using Myshop.Web.Models;

namespace Myshop.Web.IRepository
{
    public interface ICategory : IGenericRepository<Category>
    {
        public Task<Category> UpdatedWithCreatedDateAsync(Category category);
        public Task<IEnumerable<Category>> GetCategoreisIdandNames();
    }
}
