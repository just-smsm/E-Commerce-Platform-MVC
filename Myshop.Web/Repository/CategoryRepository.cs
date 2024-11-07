using Microsoft.EntityFrameworkCore;
using Myshop.Web.Data;
using Myshop.Web.IRepository;
using Myshop.Web.Models;

namespace Myshop.Web.Repository
{
    public class CategoryRepository :  GenericRepository<Category>,ICategory
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        public async Task<IEnumerable<Category>> GetCategoreisIdandNames()
        {
            return await _context.Categories
                .Select(i => new Category { Id = i.Id, Name = i.Name })
            .ToListAsync();
        }


        public async Task<Category> UpdatedWithCreatedDateAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            // Retrieve the existing category
            var existingCategory = await _context.Categories.FindAsync(category.Id);
            if (existingCategory == null)
            {
                return null; // Or handle this case as needed
            }

            // Update properties
            existingCategory.CreatedDate = DateTime.UtcNow; // Use UTC for consistency
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            // Save changes
            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();

            return existingCategory;
        }
    }
}
