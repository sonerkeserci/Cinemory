using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cinemory.Data;
using Cinemory.Models;
using Cinemory.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using static System.Formats.Asn1.AsnWriter;

namespace Cinemory.Controllers
{
    public class MoviesController : Controller
    {
        private readonly CinemoryDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MoviesController(CinemoryDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET:Index    -   sadece adminler görecek
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var cinemoryDbContext = _context.Movies
                .OrderBy(m => m.Name)
                .Include(m => m.Director);
            return View(await cinemoryDbContext.ToListAsync());
        }

        // GET:PublicIndex - kullanıcılar bu sayfayı görecek
        [AllowAnonymous]
        public async Task<IActionResult> PublicIndex()
        {
            var movies = await _context.Movies
                .Include(m => m.Director)
                .Include(m => m.Profile)
                .OrderByDescending(m => m.Name)
                .ToListAsync();

            return View(movies); // PublicIndex.cshtml adlı View dosyasına gider
        }


        // GET:Details:users will see this page as a MovieProfile
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Director)
                .Include(m => m.Genres)
                    .ThenInclude(mg => mg.Genre)
                .Include(m => m.Actors)
                    .ThenInclude(ma => ma.Actor)
                .Include(m => m.Profile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            // Ortalama puanı hesabını MovieProfile'a ekle
            var scores = await _context.Ratings
            .Where(r => r.MovieId == id)
            .Select(r => r.Score)
            .ToListAsync(); 

            double average = scores.Any() ? scores.Average() : 0;


            if (movie.Profile != null)
            {
                movie.Profile.AverageRating = Math.Round(average, 1);
            }

            // Viewbagde kullanmak için rating, review, fav, watchlist verilerini çekiyorum çünkü interact ın get kısmında işime yaraycak ufak bir iş
            var userId = _userManager.GetUserId(User);

            var rating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.MovieId == id && r.UserId == userId);

            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.MovieId == id && r.UserId == userId);

            var isFavorite = await _context.FavoriteMovies
                .AnyAsync(f => f.MovieId == id && f.UserId == userId);

            var isInWatchList = await _context.Watchlists
                .Where(w => w.UserId == userId)
                .SelectMany(w => w.Movies)
                .AnyAsync(m => m.MovieId == id);

            // ViewBag ile taşı 
            ViewBag.Rating = rating?.Score;
            ViewBag.Review = review?.Entry;
            ViewBag.IsFavorite = isFavorite;
            ViewBag.IsInWatchList = isInWatchList;

            return View(movie);


        }

        // GET:Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new MovieCreateViewModel
            {
                Directors = _context.Directors
                .OrderBy(d => d.FullName)
            .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.FullName }).ToList(),

                Genres = _context.Genres
                .OrderBy(g => g.Name)
            .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name }).ToList(),

                Actors = _context.Actors
                .OrderBy(a => a.FullName)
            .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.FullName }).ToList()
            };

            return View(model);
        }
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(MovieCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Dropdown 
                model.Directors = _context.Directors
                    .OrderBy(d => d.FullName).Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.FullName })
                    .ToList();
                model.Genres = _context.Genres
                    .OrderBy(g => g.Name).Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name }).ToList();
                model.Actors = _context.Actors
                    .OrderBy(g => g.FullName).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.FullName }).ToList();
                return View(model);
            }

            var movie = new Movie
            {
                Name = model.Name,
                Year = model.Year,
                DirectorId = model.DirectorId,
                Genres = model.SelectedGenreIds.Select(id => new MovieGenreConnection { GenreId = id }).ToList(),
                Actors = model.SelectedActorIds.Select(id => new MovieActorConnection { ActorId = id }).ToList()
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET:Edit
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.Profile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            var model = new MovieEditViewModel
            {
                Id = movie.Id,
                Name = movie.Name,
                Year = movie.Year,
                DirectorId = movie.DirectorId,
                SelectedGenreIds = movie.Genres.Select(g => g.GenreId).ToList(),
                SelectedActorIds = movie.Actors.Select(a => a.ActorId).ToList(),
                Synopsis = movie.Profile?.Synopsis,
                PosterUrl = movie.Profile?.PosterUrl,
                TrailerUrl = movie.Profile?.TrailerUrl,

                Directors = _context.Directors.OrderBy(d => d.FullName)
                    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.FullName }).ToList(),

                Genres = _context.Genres.OrderBy(g => g.Name)
                    .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name }).ToList(),

                Actors = _context.Actors.OrderBy(a => a.FullName)
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.FullName }).ToList()
            };

            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, MovieEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                model.Directors = _context.Directors.OrderBy(d => d.FullName)
                    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.FullName }).ToList();
                model.Genres = _context.Genres.OrderBy(g => g.Name)
                    .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name }).ToList();
                model.Actors = _context.Actors.OrderBy(a => a.FullName)
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.FullName }).ToList();
                return View(model);
            }

            var movie = await _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.Profile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            // Movie update
            movie.Name = model.Name;
            movie.Year = model.Year;
            movie.DirectorId = model.DirectorId;

            movie.Genres.Clear();
            movie.Genres = model.SelectedGenreIds.Select(id => new MovieGenreConnection { GenreId = id }).ToList();

            movie.Actors.Clear();
            movie.Actors = model.SelectedActorIds.Select(id => new MovieActorConnection { ActorId = id }).ToList();

            // MovieProfile update
            if (movie.Profile == null)
                movie.Profile = new MovieProfile { MovieId = movie.Id };

            movie.Profile.Synopsis = model.Synopsis;
            movie.Profile.PosterUrl = model.PosterUrl;
            movie.Profile.TrailerUrl = model.TrailerUrl;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Director)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //POST : Movie & User interaction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Interact(MovieInteractionViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var username = _userManager.GetUserName(User);

            // FAVORITE MOVIE
            if (model.IsFavorite)
            {
                if (!_context.FavoriteMovies.Any(f => f.MovieId == model.MovieId && f.UserId == userId))
                {
                    _context.FavoriteMovies.Add(new FavoriteMovie { MovieId = model.MovieId, UserId = userId });
                }
            }

            // WATCHLIST
            var watchlist = await _context.Watchlists                // Kullanıcının Watchlist'ini al
                .Include(w => w.Movies)
                .FirstOrDefaultAsync(w => w.UserId == userId);


            if (watchlist == null)              // Eğer hiç yoksa oluştur
            {

                watchlist = new Watchlist
                {
                    UserId = userId,
                    Name = $"{username}'s Watchlist",
                    Movies = new List<MovieWatchlistConnection>()
                };
                _context.Watchlists.Add(watchlist);
            }

            // Bu movie zaten listede mi?
            bool alreadyInWatchlist = watchlist.Movies?.Any(m => m.MovieId == model.MovieId) ?? false;


            // Yoksa listeye ekle
            if (!alreadyInWatchlist && model.IsInWatchlist)    //koşul böyle yazılmazsa her türlü submitte otomatik watchliste atıyor
            {
                watchlist.Movies.Add(new MovieWatchlistConnection
                {
                    MovieId = model.MovieId
                });
            }

            if (alreadyInWatchlist && !model.IsInWatchlist) // Tik işaretini kaldırınca watchlistten sil
            {
                var connection = watchlist.Movies.FirstOrDefault(m => m.MovieId == model.MovieId);
                if (connection != null)
                {
                    watchlist.Movies.Remove(connection);
                }
            }



            // RATING
            if (model.Rating.HasValue && model.Rating >= 1 && model.Rating <= 10)
            {
                var existing = await _context.Ratings.FirstOrDefaultAsync(r => r.MovieId == model.MovieId && r.UserId == userId);
                if (existing == null)
                {
                    _context.Ratings.Add(new Rating { MovieId = model.MovieId, UserId = userId, Score = model.Rating.Value });
                }
                else
                {
                    existing.Score = model.Rating.Value;
                }
            }

            // REVIEW
            if (!string.IsNullOrWhiteSpace(model.Review))
            {
                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.MovieId == model.MovieId && r.UserId == userId);

                if (existingReview == null)
                {
                    _context.Reviews.Add(new Review
                    {
                        MovieId = model.MovieId,
                        UserId = userId,
                        Entry = model.Review
                    });
                }
                else
                {
                    existingReview.Entry = model.Review;
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = model.MovieId });


        }


    }
}
