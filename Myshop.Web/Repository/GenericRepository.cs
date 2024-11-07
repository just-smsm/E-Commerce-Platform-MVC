using Microsoft.EntityFrameworkCore;
using Myshop.Web.Data;
using Myshop.Web.IRepository;
using Myshop.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Myshop.Web.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : ModelBase
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> dbSet;

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, string includeProperties = "")
        {
            IQueryable<T> query = dbSet;

            foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query.Where(predicate).ToListAsync();
        }

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }

        public async Task<T> CreateAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await dbSet.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRange(List<T> values)
        {
            dbSet.RemoveRange(values);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }
        public async Task<T> GetByIdAsync(int id, string includeProperties = "")
        {
            IQueryable<T> query = dbSet;

            // Include specified properties
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            // Retrieve the entity by ID
            return await query.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(string includeProperties = "")
        {
            IQueryable<T> query = dbSet;

            // Include specified properties
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query.ToListAsync();
        }


        // Optionally, keep the simpler GetByIdAsync method if needed
        public async Task<T> GetByIdAsync(int id)
        {
            return await dbSet.FirstOrDefaultAsync(i => i.Id == id);
        }


        public async Task<T> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
