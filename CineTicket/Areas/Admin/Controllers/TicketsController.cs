using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CineTicket.Models;

namespace CineTicket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TicketsController(ApplicationDbContext context) => _context = context;

        // ========== Danh sách ==========
        public IActionResult Index()
        {
            var tickets = _context.Tickets
                                  .Include(t => t.Showtimes)
                                      .ThenInclude(s => s.Movie)
                                  .Include(t => t.User)       // Thêm dòng này để nạp User
                                  .ToList();

            return View(tickets);
        }


        // ========== Thêm ==========
        public IActionResult Add()
        {
            LoadDropdowns();
            return View(new Ticket());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add(Ticket ticket)
        {
            // cho phép UserId để trống ⇒ xoá lỗi nếu có
            ModelState.Remove(nameof(Ticket.UserId));

            if (!ModelState.IsValid)
            {
                LoadDropdowns(ticket);
                return View(ticket);
            }

            _context.Tickets.Add(ticket);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // ========== Sửa ==========
        public IActionResult Edit(int id)
        {
            var ticket = _context.Tickets.Find(id);
            if (ticket == null) return NotFound();

            LoadDropdowns(ticket);
            return View(ticket);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Ticket ticket)
        {
            if (id != ticket.Id) return NotFound();
            ModelState.Remove(nameof(Ticket.UserId));

            if (!ModelState.IsValid)
            {
                LoadDropdowns(ticket);
                return View(ticket);
            }

            _context.Update(ticket);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // ========== Xoá ==========
        public IActionResult Delete(int id)
        {
            var ticket = _context.Tickets
                                 .Include(t => t.Showtimes)
                                     .ThenInclude(s => s.Movie)
                                 .FirstOrDefault(t => t.Id == id);
            if (ticket == null) return NotFound();
            return View(ticket);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var ticket = _context.Tickets.Find(id);
            _context.Tickets.Remove(ticket);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult DeleteAll()
        {
            var allTickets = _context.Tickets.ToList();
            _context.Tickets.RemoveRange(allTickets);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Đã xoá toàn bộ vé.";
            return RedirectToAction("Index");
        }


        // ========== Helper: nạp dropdown ==========
        private void LoadDropdowns(Ticket? selected = null)
        {
            // Suất chiếu – chỉ hiển thị “Tên phim” (đơn giản hoá)
            var showtimeList = _context.Showtimes
                                       .Include(s => s.Movie)
                                       .Select(s => new
                                       {
                                           s.Id,
                                           Display = s.Movie.Title
                                       })
                                       .ToList();

            ViewBag.Showtimes = new SelectList(showtimeList, "Id", "Display",
                                               selected?.ShowtimeId);
        }
    }
}
