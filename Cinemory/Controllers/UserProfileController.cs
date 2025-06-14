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

        /*Tüm reviewleri gösterme işi*/
        public async Task<IActionResult> UserReviews(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var reviews = await _context.Reviews
                .Include(r => r.Movie)
                .Where(r => r.UserId == user.Id)
                .ToListAsync();
            return View("UserReviews", reviews);
        }

        /*Tüm watchlistleri gösterme işi*/
        public async Task<IActionResult> UserWatchlists(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var watchlists = await _context.MovieWatchlistConnections
                .Include(m => m.Movie)
                .Include(m => m.Watchlist)
                .Where(m => m.Watchlist.UserId == user.Id)
                .ToListAsync();
            return View("UserWatchlists", watchlists);
        }

        /*Tüm favorileri gösterme işi*/
        public async Task<IActionResult> UserFavorites(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var favorites = await _context.FavoriteMovies
                .Include(f => f.Movie)
                .Where(f => f.UserId == user.Id)
                .ToListAsync();
            return View("UserFavorites", favorites);
        }

        /*Tüm ratingleri gösterme işi*/
        public async Task<IActionResult> UserMovies(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var ratings = await _context.Ratings
                .Include(r => r.Movie)
                .Where(r => r.UserId == user.Id)
                .ToListAsync();
            return View("UserMovies", ratings);
        }

    }

}
