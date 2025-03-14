using Microsoft.EntityFrameworkCore;
using Myshop.Web.Data;
using Myshop.Web.IRepository;
using Myshop.Web.Models;
using X.PagedList;

namespace Myshop.Web.Repository
{
    public class ProductRepository : GenericRepository<Product>, IProduct
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Product> GetProductByIdWithCategoryAsync(int id)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProductsWithCategoryAsync()
        {
            return await _context.Products.Include(p=>p.Category).ToListAsync();
        }

    }
}
