using Cinemory.Models.ViewModels;
using Cinemory.Models;
using Cinemory.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinemory.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly CinemoryDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public UserProfileController(CinemoryDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /*UserProfile görünümü*/
        public async Task<IActionResult> UserProfileIndex()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var userProfile = await _context.UserProfiles
                .Include(up => up.User)
                .FirstOrDefaultAsync(up => up.UserId == user.Id);

            var ratings = await _context.Ratings
                .Include(r => r.Movie)
                .Where(r => r.UserId == user.Id)
                .ToListAsync();

            var reviews = await _context.Reviews
                .Include(r => r.Movie)
                .Where(r => r.UserId == user.Id)
                .ToListAsync();

            var watchlistConnections = await _context.MovieWatchlistConnections
                .Include(m => m.Movie)
                .Include(m => m.Watchlist)
                .Where(m => m.Watchlist.UserId == user.Id)
                .ToListAsync();

            var favorites = await _context.FavoriteMovies
                .Include(f => f.Movie)
                .Where(f => f.UserId == user.Id)
                .ToListAsync();

            var movies = await _context.Movies
                .Include(m => m.Profile)
                .ToListAsync();

            var viewModel = new UserProfileViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                JoinDate = (DateTime)(userProfile?.JoinDate),
                ProfilePictureUrl = userProfile?.AvatarUrl ?? "No avatar.",
                Bio = userProfile?.Bio ?? "No bio yet.",
                TotalMoviesWatched = ratings.Count,
                TotalReviewsWritten = reviews.Count,
                UserWatchlist = watchlistConnections,
                FavoriteMovies = favorites,
                UserRatings = ratings,
                PosterUrl = movies.FirstOrDefault()?.Profile?.PosterUrl ?? "No poster.",
                UserReviews = reviews
            };

            return View("UserProfileIndex", viewModel);
        }



        /*Tüm watchlisti gösterme işi*/
        public async Task<IActionResult> UserWatchlist(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var watchlists = await _context.MovieWatchlistConnections
                .Include(m => m.Movie)
                .ThenInclude(m => m.Profile)
                .Include(m => m.Movie)
                .ThenInclude(m => m.Director)
                .Include(m => m.Watchlist)
                .Where(m => m.Watchlist.UserId == user.Id)
                .ToListAsync();


            var avgDict = await _context.Ratings
                .GroupBy(r => r.MovieId)
                .Select(g => new { g.Key, Avg = g.Average(r => r.Score) })
                .ToDictionaryAsync(x => x.Key, x => x.Avg);

            // her film için ortalama rating'i hesapla ve profile'a koy
            foreach (var conn in watchlists)
            {
                if (conn.Movie?.Profile != null && avgDict.TryGetValue(conn.MovieId, out double avg))
                {
                    conn.Movie.Profile.AverageRating = Math.Round(avg, 1);
                }
            }



            return View("UserWatchlist", watchlists);
        }

        /*Tüm favorileri gösterme işi*/
        public async Task<IActionResult> UserFavorites(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var favorites = await _context.FavoriteMovies
                .Include(f => f.Movie)
                .ThenInclude(m => m.Profile)
                .Where(f => f.UserId == user.Id)
                .ToListAsync();

            var avgDict = await _context.Ratings
                .GroupBy(r => r.MovieId)
                .Select(g => new { g.Key, Avg = g.Average(r => r.Score) })
                .ToDictionaryAsync(x => x.Key, x => x.Avg);

            var ratings = await _context.Ratings
                .Include(r => r.Movie)
                .ThenInclude(m => m.Profile)
                .Include(r => r.Movie)
                .ThenInclude(m => m.Director)
                .Where(r => r.UserId == user.Id)
                .ToListAsync();
            foreach (var rating in ratings)
            {
                if (rating.Movie?.Profile != null && avgDict.TryGetValue(rating.MovieId, out double avg))
                {
                    rating.Movie.Profile.AverageRating = avg;
                }
            }

            return View("UserFavorites", favorites);
        }

        /*Tüm ratingleri gösterme işi*/
        public async Task<IActionResult> UserMovies(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var ratings = await _context.Ratings
                .Include(r => r.Movie)
                .ThenInclude(m => m.Profile)
                .Include(r => r.Movie)
                .ThenInclude(m => m.Director)
                .Where(r => r.UserId == user.Id)
                .ToListAsync();

            // rating'leri movieId'ye göre gruplayıp ortalama hesapbı
            var avgDict = await _context.Ratings
                .GroupBy(r => r.MovieId)
                .Select(g => new { g.Key, Avg = g.Average(r => r.Score) })
                .ToDictionaryAsync(x => x.Key, x => x.Avg);
            foreach (var rating in ratings)
            {
                if (rating.Movie?.Profile != null && avgDict.TryGetValue(rating.MovieId, out double avg))
                {
                    rating.Movie.Profile.AverageRating = avg;
                }
            }

            return View("UserMovies", ratings);
        }

        /*Tüm reviewleri gösterme işi*/
        public async Task<IActionResult> UserReviews(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var reviews = await _context.Reviews
                .Include(r => r.Movie)
                .ThenInclude(m => m.Profile)
                .Where(r => r.UserId == user.Id)
                .ToListAsync();
            return View("UserReviews", reviews);
        }

    }

}
