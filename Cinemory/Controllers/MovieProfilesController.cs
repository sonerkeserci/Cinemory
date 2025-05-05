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
    public class MovieProfilesController : Controller
    {
        private readonly CinemoryDbContext _context;

        public MovieProfilesController(CinemoryDbContext context)
        {
            _context = context;
        }

        // GET: MovieProfiles
        public async Task<IActionResult> Index()
        {
            var cinemoryDbContext = _context.MovieProfiles.Include(m => m.Movie);
            return View(await cinemoryDbContext.ToListAsync());
        }

        // GET: MovieProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieProfile = await _context.MovieProfiles
                .Include(m => m.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movieProfile == null)
            {
                return NotFound();
            }

            return View(movieProfile);
        }

        // GET: MovieProfiles/Create
        public IActionResult Create()
        {
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id");
            return View();
        }

        // POST: MovieProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Synopsis,PosterUrl,TrailerUrl,MovieId")] MovieProfile movieProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movieProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id", movieProfile.MovieId);
            return View(movieProfile);
        }

        // GET: MovieProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieProfile = await _context.MovieProfiles.FindAsync(id);
            if (movieProfile == null)
            {
                return NotFound();
            }
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id", movieProfile.MovieId);
            return View(movieProfile);
        }

        // POST: MovieProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Synopsis,PosterUrl,TrailerUrl,MovieId")] MovieProfile movieProfile)
        {
            if (id != movieProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movieProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieProfileExists(movieProfile.Id))
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
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id", movieProfile.MovieId);
            return View(movieProfile);
        }

        // GET: MovieProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieProfile = await _context.MovieProfiles
                .Include(m => m.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movieProfile == null)
            {
                return NotFound();
            }

            return View(movieProfile);
        }

        // POST: MovieProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movieProfile = await _context.MovieProfiles.FindAsync(id);
            if (movieProfile != null)
            {
                _context.MovieProfiles.Remove(movieProfile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieProfileExists(int id)
        {
            return _context.MovieProfiles.Any(e => e.Id == id);
        }
    }
}
