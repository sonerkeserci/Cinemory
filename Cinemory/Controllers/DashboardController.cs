using Microsoft.AspNetCore.Mvc;

namespace Cinemory.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
