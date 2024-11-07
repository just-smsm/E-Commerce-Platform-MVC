using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Myshop.Web.Models;
using System.Linq.Expressions;

namespace Myshop.Web.IRepository
{
    public interface IOrderDetail:IGenericRepository<OrderDetail>
    {
        public Task<List<OrderDetail>> GetAllWithProduct();
        Task<OrderDetail> GetFirstOrDefaultAsync(Expression<Func<OrderDetail, bool>> filter);
        public Task<List<OrderDetail>> GetOrderWithAppUser(int id);
    }
}
