using Microsoft.AspNetCore.Mvc;

namespace Cinemory.Controllers
{
    public class UserFeedController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
