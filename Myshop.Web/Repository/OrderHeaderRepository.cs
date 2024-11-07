using Microsoft.EntityFrameworkCore;
using Myshop.Web.Data;
using Myshop.Web.IRepository;
using Myshop.Web.Models;

namespace Myshop.Web.Repository
{
    public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeader
    {
        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<OrderHeader> GetOrderWithAppUser(int Id)
        {
            return await _context.OrderHeaders.Include(a=>a.AppUser).FirstOrDefaultAsync(i => i.Id == Id);
        }

        async Task IOrderHeader.UpdateOrderStatus(int Id, string OrderStatus, string PaymentStatus)
        {
            var order = await _context.OrderHeaders.FirstAsync(x => x.Id == Id);
            if (order.OrderStatus != null)
            {
                order.OrderStatus = OrderStatus;
                if (order.PaymentStatus != PaymentStatus)
                {
                    order.PaymentStatus = PaymentStatus;
                }
            }
 
            await _context.SaveChangesAsync();
        }
    }

}
