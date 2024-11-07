using Myshop.Web.Data;
using Myshop.Web.Models;

namespace Myshop.Web.Service
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AppUser> GetUserByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task UpdateUserAsync(AppUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
