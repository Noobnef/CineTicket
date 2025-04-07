
using CineTicket.Models;
using CineTicket.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CineTicket.Areas.Admin.Controllers
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
        }// GET: Showtimes/Add
        public IActionResult Add()
        {
            LoadDropdowns();

            // StartTime mặc định = thời điểm hiện tại
            var model = new Showtime
            {
                StartTime = DateTime.Now
            };

            return View(model);
        }

        /*--------------------------------------  ADD (POST) */
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Showtime showtime)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns(showtime);
                return View(showtime);
            }

            _context.Add(showtime);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thêm suất chiếu thành công!";
            return RedirectToAction(nameof(Index));
        }
        private void LoadDropdowns(Showtime selected = null)
        {
            ViewBag.Movies = new SelectList(
                _context.Movies.OrderBy(m => m.Title),
                "Id", "Title",
                selected?.MovieId);

            ViewBag.Rooms = new SelectList(
                _context.Rooms.OrderBy(r => r.Name),
                "Id", "Name",
                selected?.RoomId);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null) return NotFound();

            LoadDropdowns(showtime);
            return View(showtime);
        }

        /*--------------------------------------  EDIT (POST) */
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Showtime showtime)
        {
            if (id != showtime.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                LoadDropdowns(showtime);
                return View(showtime);
            }

            try
            {
                _context.Update(showtime);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật suất chiếu thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Showtimes.Any(e => e.Id == id)) return NotFound();
                throw;
            }
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
