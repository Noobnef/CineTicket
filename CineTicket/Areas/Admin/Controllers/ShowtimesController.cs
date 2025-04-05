
using CineTicket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CineTicket.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ShowtimesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShowtimesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var showtimes = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .ToListAsync();
            return View(showtimes);
        }

        public IActionResult Add()
        {
            ViewBag.Movies = new SelectList(_context.Movies, "Id", "Title");
            ViewBag.Rooms = new SelectList(_context.Rooms, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Showtime showtime)
        {
            if (ModelState.IsValid)
            {
                _context.Showtimes.Add(showtime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(showtime);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null) return NotFound();

            ViewBag.Movies = new SelectList(_context.Movies, "Id", "Title", showtime.MovieId);
            ViewBag.Rooms = new SelectList(_context.Rooms, "Id", "Name", showtime.RoomId);
            return View(showtime);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Showtime showtime)
        {
            if (id != showtime.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(showtime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Movies = new SelectList(_context.Movies, "Id", "Title", showtime.MovieId);
            ViewBag.Rooms = new SelectList(_context.Rooms, "Id", "Name", showtime.RoomId);
            return View(showtime);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null) return NotFound();

            _context.Showtimes.Remove(showtime);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
