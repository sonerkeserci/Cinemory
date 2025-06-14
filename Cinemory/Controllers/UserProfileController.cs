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


    }

}
