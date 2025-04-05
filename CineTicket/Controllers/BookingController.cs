using Microsoft.AspNetCore.Mvc;
using CineTicket.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;

public class BookingController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index(int movieId)
    {
        var movie = _context.Movies.FirstOrDefault(m => m.Id == movieId);
        if (movie == null)
        {
            return NotFound();
        }

        var showtime = _context.Showtimes.FirstOrDefault(s => s.MovieId == movieId);
        if (showtime == null)
        {
            return BadRequest("Không có suất chiếu cho phim này.");
        }

        var booking = new BookingViewModel
        {
            MovieId = movie.Id,
            MovieTitle = movie.Title,
            TicketPrice = 100000,
            ShowtimeId = showtime.Id // Gán suất chiếu có thật!
        };

        return View(booking);
    }


    [HttpPost]
    public IActionResult ConfirmBooking(BookingViewModel model)
    {
        if (string.IsNullOrEmpty(model.SeatNumbers))
            return RedirectToAction("Index", new { movieId = model.MovieId });

        var seats = model.SeatNumbers.Split(',');

        foreach (var seat in seats)
        {
            var ticket = new Ticket
            {
                ShowtimeId = model.ShowtimeId, // dùng ShowtimeId từ viewmodel
                SeatNumber = seat,
                Price = model.TicketPrice,
                UserId = User.Identity.IsAuthenticated ? _userManager.GetUserId(User) : null
            };


            _context.Tickets.Add(ticket);
        }

        _context.SaveChanges();
        return RedirectToAction("Success");
    }

    public IActionResult Success()
    {
        return View();
    }
}
