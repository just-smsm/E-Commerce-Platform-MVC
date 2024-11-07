using Microsoft.AspNetCore.Mvc;
using Myshop.Web.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Myshop.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Myshop.Web.Service;

namespace Myshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public UsersController(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return Unauthorized();
            }

            string userId = claim.Value;
            var users = await _context.Users
                .Where(x => x.Id != userId)
                .Select(x => new UserViewModel
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Email = x.Email,
                    LockoutEnd = x.LockoutEnd
                })
                .ToListAsync();

            return View(users);
        }

        public async Task<IActionResult> LockUnlock(string id)
        {

            // Fetch the user by Id
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Check the lockout status
            if (user.LockoutEnd == null || user.LockoutEnd < DateTime.Now)
            {
                // Lock the user
                user.LockoutEnd = DateTime.Now.AddYears(1); // Example: lock for 1 year
            }
            else
            {
                // Unlock the user
                user.LockoutEnd = null;
            }

            await _userService.UpdateUserAsync(user); // Ensure to save changes

            return RedirectToAction("Index", "Users", new {area = "Admin"}); // Redirect back to the user list
        }
    }
}
