using Microsoft.AspNetCore.Mvc;

namespace Cinemory.Controllers
{
    public class UserProfileController : Controller
    {
        public IActionResult UserProfileIndex()
        {
            return View();
        }
    }
}
