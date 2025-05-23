using Microsoft.AspNetCore.Mvc;
using Cinemory.Models.ViewModels;
using Cinemory.Models;
using Cinemory.Data;
using Microsoft.AspNetCore.Identity;


namespace Cinemory.Controllers
{
    public class UserFeedController : Controller
    {
        private readonly CinemoryDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public UserFeedController(CinemoryDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: UserFeed
        public async Task<IActionResult> UserFeedIndex(UserFeedViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var userId = user.Id;

           



            return View(model);
        }
    }
}
