using Myshop.Web.Models;

namespace Myshop.Web.Service
{
    public interface IUserService
    {
        Task<AppUser> GetUserByIdAsync(string id);
        Task UpdateUserAsync(AppUser user);

    }
}
