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

    public IActionResult Index(int movieId, int? showtimeId)
    {
        var movie = _context.Movies.FirstOrDefault(m => m.Id == movieId);
        if (movie == null)
        {
            return NotFound();
        }

        // Nếu chưa truyền showtimeId (chưa chọn suất chiếu)
        // -> Lấy danh sách suất chiếu cho phim này để người dùng chọn
        if (!showtimeId.HasValue)
        {
            var showtimes = _context.Showtimes
                .Where(s => s.MovieId == movieId)
                .OrderBy(s => s.StartTime)
                .ToList();

            if (!showtimes.Any())
            {
                return BadRequest("Không có suất chiếu cho phim này.");
            }

            // Trả về View cho phép người dùng chọn suất chiếu
            return View("ChooseShowtime", new ChooseShowtimeViewModel
            {
                MovieId = movie.Id,
                MovieTitle = movie.Title,
                Showtimes = showtimes
            });
        }

        // Nếu đã có showtimeId => Lấy suất chiếu để chuẩn bị đặt vé
        var selectedShowtime = _context.Showtimes
            .FirstOrDefault(s => s.Id == showtimeId.Value && s.MovieId == movieId);
        if (selectedShowtime == null)
        {
            return BadRequest("Suất chiếu không hợp lệ.");
        }

        // Tạo ViewModel booking
        var booking = new BookingViewModel
        {
            MovieId = movie.Id,
            MovieTitle = movie.Title,
            TicketPrice = 100000,
            ShowtimeId = selectedShowtime.Id
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
