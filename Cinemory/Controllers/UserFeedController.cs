using Microsoft.AspNetCore.Mvc;
using Cinemory.Models.ViewModels;
using Cinemory.Models;
using Cinemory.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


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
        public async Task<IActionResult> UserFeedIndex()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var userId = user.Id;

            /*Last watched movies...*/
            var lastWatched = await _context.Ratings
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.DateRated)             //UserId üzerinden kullanıcın Rate lediği filmleri alıp sondan başa sıralama
            .Include(r => r.Movie)
                .ThenInclude(m => m.Profile)
            .Include(r => r.Movie)
                .ThenInclude(m => m.Reviews)
            .Include(r => r.Movie)
                .ThenInclude(m => m.FavoritedByUsers)       //ne var ne yok getiriyoruz *** razorda işe yarayacak
            .Include(r => r.Movie)
                .ThenInclude(m => m.Watchlists)
                    .ThenInclude(w => w.Watchlist)
            .OrderByDescending (r => r.DateRated)          //son izlenenleri alıyoruz
            .Take(5)                                 //son 5 filmi alıyoruz
            .Select(r => new MovieInteractionViewModel
            {
            
                MovieId = r.MovieId,
                Name = r.Movie.Name,
                Year = r.Movie.Year,
                PosterUrl = r.Movie.Profile.PosterUrl,
                AverageRating = r.Movie.Profile.AverageRating,
                Rating = r.Score,            
                Review = r.Movie.Reviews                    //aldıklarımızı ViewModel'e mapliyoruz
                .Where(rv => rv.UserId == userId)
                .Select(rv => rv.Entry)
                .FirstOrDefault() ?? "",

                IsFavorite = r.Movie.FavoritedByUsers.Any(f => f.UserId == userId),
                IsInWatchlist = r.Movie.Watchlists.Any(w => w.Watchlist.UserId == userId)


            })
            .ToListAsync();

            /*Last watchlisted movies...*/
            var watchlisted = await _context.MovieWatchlistConnections
            .Where(c => c.Watchlist.UserId == userId)
            .Include(c => c.Movie)
                .ThenInclude(m => m.Profile)
            .Include(c => c.Movie)
                .ThenInclude(m => m.Reviews)
            .Include(c => c.Movie)
                .ThenInclude(m => m.FavoritedByUsers)
            .Include(c => c.Movie)
                .ThenInclude(m => m.Watchlists)
                    .ThenInclude(w => w.Watchlist)
                    .OrderByDescending(c => c.DateAdded) // Watchlist üzerinden son eklenenleri alıyoruz
                    .Take(5) 
            .Select(c => new MovieInteractionViewModel
            {
                MovieId = c.MovieId,
                Name = c.Movie.Name,
                Year = c.Movie.Year,
                PosterUrl = c.Movie.Profile.PosterUrl,
                AverageRating = c.Movie.Profile.AverageRating,

                Rating = c.Movie.Ratings
                    .Where(r => r.UserId == userId)
                    .Select(r => r.Score)
                    .FirstOrDefault(),

                Review = c.Movie.Reviews
                    .Where(rv => rv.UserId == userId)
                    .Select(rv => rv.Entry)
                    .FirstOrDefault() ?? "",

                IsFavorite = c.Movie.FavoritedByUsers.Any(f => f.UserId == userId),
                IsInWatchlist = true
            })
            .ToListAsync();

            /*Recently added movies...*/
            var recentlyAdded = await _context.Movies
            .OrderByDescending(m => m.Id) // Varsa CreatedAt ile değiştir
            .Take(5)
            .Include(m => m.Profile)
            .Include(m => m.Reviews)
            .Include(m => m.FavoritedByUsers)
            .Include(m => m.Watchlists)
                .ThenInclude(w => w.Watchlist)
            .Select(m => new MovieInteractionViewModel
            {
                MovieId = m.Id,
                Name = m.Name,
                Year = m.Year,
                PosterUrl = m.Profile.PosterUrl,
                AverageRating = m.Profile.AverageRating,

                Rating = m.Ratings
                    .Where(r => r.UserId == userId)
                    .Select(r => r.Score)
                    .FirstOrDefault(),

                Review = m.Reviews
                    .Where(rv => rv.UserId == userId)
                    .Select(rv => rv.Entry)
                    .FirstOrDefault() ?? "",

                IsFavorite = m.FavoritedByUsers.Any(f => f.UserId == userId),
                IsInWatchlist = m.Watchlists.Any(w => w.Watchlist.UserId == userId)
            })
            .ToListAsync();

            /*Recommended for you... Random for now */
            var recommended = await _context.Movies
            .OrderBy(m => Guid.NewGuid()) // rastgele
            .Take(5)
            .Include(m => m.Profile)
            .Include(m => m.Reviews)
            .Include(m => m.FavoritedByUsers)
            .Include(m => m.Watchlists)
            .ThenInclude(w => w.Watchlist)
            .Select(m => new MovieInteractionViewModel
            {
            MovieId = m.Id,
            Name = m.Name,
            Year = m.Year,
            PosterUrl = m.Profile.PosterUrl,
            AverageRating = m.Profile.AverageRating,

            Rating = m.Ratings
            .Where(r => r.UserId == userId)
            .Select(r => r.Score)
            .FirstOrDefault(),

            Review = m.Reviews
            .Where(rv => rv.UserId == userId)
            .Select(rv => rv.Entry)
            .FirstOrDefault() ?? "",

            IsFavorite = m.FavoritedByUsers.Any(f => f.UserId == userId),
            IsInWatchlist = m.Watchlists.Any(w => w.Watchlist.UserId == userId)
            })
            .ToListAsync();


            var model = new UserFeedViewModel
            {
                LastWatched = lastWatched,
                Watchlisted = watchlisted,
                RecentlyAdded = recentlyAdded,
                Recommended = recommended,
                RssWidgetUrl = "https://www.tasteofcinema.com/category/lists/film-lists/"
            };

            return View(model);
        }
    }
}
