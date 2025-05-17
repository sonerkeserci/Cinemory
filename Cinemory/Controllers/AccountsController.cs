using Cinemory.Data;
using Cinemory.Models;
using Cinemory.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

                return RedirectToAction("Index", "Movies");
            }


            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }


    }
}




