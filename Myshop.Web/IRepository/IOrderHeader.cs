using Myshop.Web.Models;

namespace Myshop.Web.IRepository
{
    public interface IOrderHeader:IGenericRepository<OrderHeader>
    {
        Task UpdateOrderStatus(int Id, string OrderStatus, string PaymentStatus);
        Task<OrderHeader> GetOrderWithAppUser(int Id);
    }
}
