using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Myshop.Web.Data;
using Myshop.Web.IRepository;
using Myshop.Web.Models;
using System.Linq.Expressions;

namespace Myshop.Web.Repository
{
    public class OrderDetailsRepository : GenericRepository<OrderDetail>, IOrderDetail
    {
        public OrderDetailsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<OrderDetail>> GetAllWithProduct()
        {
            return await _context.OrderDetails.Include(x => x.Product).ToListAsync();
        }
        public async Task<List<OrderDetail>> GetOrderWithAppUser(int id)
        {
            return await _context.OrderDetails
                .Where(a => a.OrderHeaderId == id)
                .Include(a => a.OrderHeader.AppUser)
                .Include(a => a.Product)
                .ToListAsync();
        }

        public async Task<OrderDetail> GetFirstOrDefaultAsync(Expression<Func<OrderDetail, bool>> filter)
        {
            return await _context.OrderDetails
                .Where(filter)
                .FirstOrDefaultAsync();
        }
    }
}
