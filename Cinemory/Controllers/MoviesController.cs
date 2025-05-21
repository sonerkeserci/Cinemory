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

        // GET:Index:only admin can see this page
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var cinemoryDbContext = _context.Movies
                .OrderBy(m => m.Name)
                .Include(m => m.Director);
            return View(await cinemoryDbContext.ToListAsync());
        }

        // GET:Details:users will see this page
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

       

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
