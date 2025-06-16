using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Cinemory.Data;
using Cinemory.Models;
using Cinemory.Models.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace Cinemory.Controllers
{
    
    public class DashboardController : Controller
    {

        private readonly CinemoryDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public DashboardController(CinemoryDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Lists()
        {
            var model = await _context.Watchlists
                .Include(w => w.User)
                    .ThenInclude(u => u.Profile) // Kullanıcı profili bilgilerini dahil et
                .Include(w => w.Movies)
                    .ThenInclude(mwc => mwc.Movie)
                        .ThenInclude(m => m.Profile)
                        .Select(w => new WatchlistPreviewModel
                        {
                            WatchlistId = w.Id,
                            WatchlistName = w.Name,
                            UserId = w.UserId,
                            UserName = w.User.UserName,
                            TotalMovies = w.Movies.Count,
                            PreviewMovies = w.Movies
                                .Select(mwc => mwc.Movie)
                                .Where(m => m != null && m.Profile != null)
                                .Take(3)
                                .ToList()

                        })
                .ToListAsync();

            return View(model);
        }

    }
}
