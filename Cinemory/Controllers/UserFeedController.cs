using Microsoft.AspNetCore.Mvc;
using Cinemory.Models.ViewModels;

namespace Cinemory.Controllers
{
    public class UserFeedController : Controller
    {
        public IActionResult UserFeedIndex()
        {

            return View();
        }
    }
}
