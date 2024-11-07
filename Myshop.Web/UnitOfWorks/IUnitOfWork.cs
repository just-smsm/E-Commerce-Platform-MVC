using Myshop.Web.IRepository;
using Myshop.Web.Models;

namespace Myshop.Web.UnitOfWorks
{
    public interface IUnitOfWork
    {
        IProduct Product { get; }
        ICategory Category { get; }
        IShoppingCart ShoppingCart { get; }
        IOrderDetail OrderDetail { get; }
        IOrderHeader OrderHeader { get; }
        Task<int> CompleteAsync();
        void Dispose();
    }
}
