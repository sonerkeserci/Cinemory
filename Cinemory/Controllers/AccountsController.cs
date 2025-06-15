using System.Threading.Tasks;
using Cinemory.Data;
using Cinemory.Models;
using Cinemory.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinemory.Controllers
{
    public class AccountsController : Controller
    {
        private readonly CinemoryDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountsController(CinemoryDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult AccessDenied()
        {
            return View();
        }


        // GET: Register    
        [HttpGet]
        public IActionResult RegisterPartial()
        {
            return PartialView("_RegisterPartial", new RegisterViewModel());
        }


        // POST: Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {

                return PartialView("_RegisterPartial", model);
            }

            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {

                var profile = new UserProfile   // UserProfile oluşturma işi
                {
                    UserId = user.Id,
                    JoinDate = DateTime.UtcNow
                };

                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync();


                await _signInManager.SignInAsync(user, isPersistent: false);  // otomatik sign-in işi

                return Json(new { success = true, redirectUrl = Url.Action("UserFeedIndex", "UserFeed") });

            }


            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return PartialView("_RegisterPartial", model);

        }

        [HttpGet]
        public async Task<IActionResult> Members()
        {
            var users = await _context.Users
                .Include(u => u.Profile) // UserProfile ile ilişkiyi dahil et
                .ToListAsync();
            if (users == null)
            {
                return NotFound();
            }


            return View(users);
        }

        // GET: Login
        [HttpGet]
        public IActionResult LoginPartial()
        {
            return PartialView("_LoginPartial", new LoginViewModel());
        }

        // POST: Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model) //getten gelen model içinde sadece user ve password var
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_LoginPartial", model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);   //öyle bir user yoksa
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return PartialView("_LoginPartial", model);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false); //varsa password kontrolü
            if (result.Succeeded)
            {

                await _signInManager.SignInAsync(user, isPersistent: false); // ✨ bu şart
                string? redirectUrl;

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    redirectUrl = Url.Action("Index", "Dashboard");
                else
                    redirectUrl = Url.Action("UserFeedIndex", "UserFeed");

                return Json(new { success = true,redirectUrl });
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return PartialView("_LoginPartial", model);
        }


        // POST: Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Landing", "Home"); // veya hangi controller/view'a döneceksen
        }









    }
}




