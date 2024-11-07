using Microsoft.AspNetCore.Mvc;
using Myshop.Web.Models;
using System.Linq.Expressions;

namespace Myshop.Web.IRepository
{
    public interface IGenericRepository<T> where T : ModelBase
    {
       
        public  Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, string includeProperties = "");
        public  Task<T> GetByIdAsync(int id, string includeProperties = "");
        public Task<IEnumerable<T>> GetAllAsync(string includeProperties = "");
        
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<bool> DeleteRange(List<T> values);
        Task<T> GetByIdAsync(int id);
    }
}
