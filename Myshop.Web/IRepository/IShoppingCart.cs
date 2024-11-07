using Myshop.Web.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Myshop.Web.IRepository
{
    public interface IShoppingCart : IGenericRepository<ShoppingCart>
    {
        Task<IEnumerable<ShoppingCart>> GetAllAsync(Expression<Func<ShoppingCart, bool>> predicate, string includeProperties = null);
        public int UpdateCount(ShoppingCart cart, int Count);
        public  Task<ShoppingCart> GetFirstOrDefaultAsync(string appId, int productId);
    }
}
