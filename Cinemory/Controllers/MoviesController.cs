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

namespace Cinemory.Controllers
{
    public class MoviesController : Controller
    {
        private readonly CinemoryDbContext _context;

        public MoviesController(CinemoryDbContext context)
        {
            _context = context;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            var cinemoryDbContext = _context.Movies.Include(m => m.Director);
            return View(await cinemoryDbContext.ToListAsync());
        }

        // GET
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
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET:Create
        public IActionResult Create()
        {
            var model = new MovieCreateViewModel
            {
                Directors = _context.Directors
            .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.FullName }).ToList(),

                Genres = _context.Genres
            .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name }).ToList(),

                Actors = _context.Actors
            .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.FullName }).ToList()
            };

            return View(model);
        }
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Dropdown 
                model.Directors = _context.Directors.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.FullName }).ToList();
                model.Genres = _context.Genres.Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name }).ToList();
                model.Actors = _context.Actors.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.FullName }).ToList();
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            ViewData["DirectorId"] = new SelectList(_context.Directors, "Id", "Id", movie.DirectorId);
            return View(movie);
        }
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Year,DirectorId")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DirectorId"] = new SelectList(_context.Directors, "Id", "Id", movie.DirectorId);
            return View(movie);
        }

        // GET: Delete
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
