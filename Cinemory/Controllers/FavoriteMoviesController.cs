using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cinemory.Data;
using Cinemory.Models;

namespace Cinemory.Controllers
{
    public class FavoriteMoviesController : Controller
    {
        private readonly CinemoryDbContext _context;

        public FavoriteMoviesController(CinemoryDbContext context)
        {
            _context = context;
        }

        // GET: FavoriteMovies
        public async Task<IActionResult> Index()
        {
            var cinemoryDbContext = _context.FavoriteMovies.Include(f => f.Movie).Include(f => f.User);
            return View(await cinemoryDbContext.ToListAsync());
        }

        // GET: FavoriteMovies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favoriteMovie = await _context.FavoriteMovies
                .Include(f => f.Movie)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (favoriteMovie == null)
            {
                return NotFound();
            }

            return View(favoriteMovie);
        }

        // GET: FavoriteMovies/Create
        public IActionResult Create()
        {
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: FavoriteMovies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,MovieId")] FavoriteMovie favoriteMovie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(favoriteMovie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id", favoriteMovie.MovieId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", favoriteMovie.UserId);
            return View(favoriteMovie);
        }

        // GET: FavoriteMovies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favoriteMovie = await _context.FavoriteMovies.FindAsync(id);
            if (favoriteMovie == null)
            {
                return NotFound();
            }
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id", favoriteMovie.MovieId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", favoriteMovie.UserId);
            return View(favoriteMovie);
        }

        // POST: FavoriteMovies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,MovieId")] FavoriteMovie favoriteMovie)
        {
            if (id != favoriteMovie.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(favoriteMovie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FavoriteMovieExists(favoriteMovie.UserId))
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
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id", favoriteMovie.MovieId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", favoriteMovie.UserId);
            return View(favoriteMovie);
        }

        // GET: FavoriteMovies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favoriteMovie = await _context.FavoriteMovies
                .Include(f => f.Movie)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (favoriteMovie == null)
            {
                return NotFound();
            }

            return View(favoriteMovie);
        }

        // POST: FavoriteMovies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var favoriteMovie = await _context.FavoriteMovies.FindAsync(id);
            if (favoriteMovie != null)
            {
                _context.FavoriteMovies.Remove(favoriteMovie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FavoriteMovieExists(int id)
        {
            return _context.FavoriteMovies.Any(e => e.UserId == id);
        }
    }
}
