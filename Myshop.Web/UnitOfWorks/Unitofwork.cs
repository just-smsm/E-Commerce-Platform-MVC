using Myshop.Web.Data;
using Myshop.Web.IRepository;
using Myshop.Web.Repository;

namespace Myshop.Web.UnitOfWorks
{
    public class UnitOfwork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        public IProduct Product { get; private set; }
        public ICategory Category { get; private set; }

        public IShoppingCart ShoppingCart { get; private set; }

        public IOrderDetail OrderDetail { get; private set; }

        public IOrderHeader OrderHeader { get; private set; }

        public UnitOfwork(ApplicationDbContext context)
        {
            _context = context;
            Product = new ProductRepository(_context);
            Category = new CategoryRepository(_context);
            ShoppingCart = new ShoppingCartRepository(_context);
            OrderDetail =new OrderDetailsRepository(_context);
            OrderHeader =new OrderHeaderRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
            
        }
        

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

