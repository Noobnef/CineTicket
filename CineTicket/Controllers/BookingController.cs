using Microsoft.AspNetCore.Mvc;
using CineTicket.Models; // Import models
using System.Linq;

public class BookingController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookingController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Trang đặt vé theo ID phim
    public IActionResult Index(int movieId)
    {
        var movie = _context.Movies.FirstOrDefault(m => m.Id == movieId);
        if (movie == null)
        {
            return NotFound();
        }

        var booking = new BookingViewModel
        {
            MovieId = movie.Id,
            MovieTitle = movie.Title,
            TicketPrice = 100000 // Giá mặc định (có thể lấy từ DB)
        };

        return View(booking);
    }

    [HttpPost]
    public IActionResult ConfirmBooking(BookingViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        // Xử lý lưu thông tin đặt vé vào database (chưa làm)
        return RedirectToAction("Success");
    }

    public IActionResult Success()
    {
        return View();
    }
}
