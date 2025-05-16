using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Cinemory.Models;


namespace Cinemory.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;


        public HomeController(UserManager<AppUser> userManager)    //dependency injection ile UserManager servisi
        {
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()  // asenkron... bu siteye ilk girildiğinde çalışır
        {

            if (User?.Identity?.IsAuthenticated != true) // giriş yapmamışsa "Landing.cshtml" view'ını döndür
                return View("Landing");


            var user = await _userManager.GetUserAsync(User); // yapmışsa, UserManager üzerinden login olan kullanıcıyı getir


            if (user != null && await _userManager.IsInRoleAsync(user, "Admin")) // kullanıcı "Admin" rolündeyse Dashboard
                return RedirectToAction("Index", "Dashboard");


            return RedirectToAction("ActivityFeed", "UserFeed"); // Admin değilse, normal kullanıcı feed sayfası
        }

        // Github Test
        public IActionResult Landing()
        {
            return View();
        }
    }
}
