using Microsoft.EntityFrameworkCore;
using Myshop.Web.Data;
using Myshop.Web.IRepository;
using Myshop.Web.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using NuGet.ContentModel;
using Microsoft.AspNetCore.Mvc;

namespace Myshop.Web.Repository
{
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCart
    {
        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ShoppingCart>> GetAllAsync(Expression<Func<ShoppingCart, bool>> predicate, string includeProperties = null)
        {
            IQueryable<ShoppingCart> query = dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<ShoppingCart> GetFirstOrDefaultAsync(string appId, int productId)
        {
            return await _context.ShoppingCarts
                                 .Include(i => i.Product)
                                 .FirstOrDefaultAsync(i => i.AppUserId == appId && i.ProductId == productId);
        }


        public int UpdateCount(ShoppingCart cart, int Count)
        {
            cart.Count += Count;
            return cart.Count;
        }

        
    }
}
