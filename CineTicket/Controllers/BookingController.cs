using Microsoft.AspNetCore.Mvc;
using CineTicket.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            return NotFound("Không tìm thấy phim.");
        }

        // Nếu chưa chọn suất chiếu => hiển thị trang ChooseShowtime
        if (!showtimeId.HasValue)
        {
            var showtimes = _context.Showtimes
                .Where(s => s.MovieId == movieId && s.StartTime > DateTime.Now)
                .OrderBy(s => s.StartTime)
                .ToList();

            if (!showtimes.Any())
            {
                return BadRequest("Phim này chưa có suất chiếu.");
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
            .Include(s => s.Room) // cần Include để lấy Room
            .FirstOrDefault(s => s.Id == showtimeId.Value && s.MovieId == movieId);
        if (selectedShowtime == null)
        {
            return BadRequest("Suất chiếu không hợp lệ.");
        }

        // Lấy các ghế đã đặt cho suất chiếu này
        var bookedSeats = _context.Tickets
            .Where(t => t.ShowtimeId == selectedShowtime.Id)
            .Select(t => t.SeatNumber)
            .ToList();

        // Tạo ViewModel
        var booking = new BookingViewModel
        {
            MovieId = movie.Id,
            MovieTitle = movie.Title,
            //TicketPrice = 100000,      // tạm thời cố định

            TicketPrice = selectedShowtime.Room.TicketPrice, // Lấy từ DB thay vì gán cứng
            ShowtimeId = selectedShowtime.Id,

            // Danh sách ghế đã đặt
            AlreadyBookedSeats = bookedSeats
        };

        // Trả về View hiển thị sơ đồ ghế
        return View(booking);
    }

    [HttpPost]
    public IActionResult ConfirmBooking(BookingViewModel model)
    {
        // Kiểm tra nếu chưa chọn ghế
        if (string.IsNullOrEmpty(model.SeatNumbers))
        {
            // Quay lại trang Index để chọn lại
            return RedirectToAction("Index", new { movieId = model.MovieId, showtimeId = model.ShowtimeId });
        }

        // Tách danh sách ghế
        var seats = model.SeatNumbers.Split(',');

        // Lưu thông tin vé cho từng ghế
        foreach (var seat in seats)
        {
            var ticket = new Ticket
            {
                ShowtimeId = model.ShowtimeId,
                SeatNumber = seat,
                Price = model.TicketPrice,
                BookingTime = DateTime.Now,
                UserId = User.Identity.IsAuthenticated
                    ? _userManager.GetUserId(User)
                    : null
            };
            _context.Tickets.Add(ticket);
        }

        _context.SaveChanges();

        // Sau khi đặt thành công => quay lại trang Index 
        // để xem ghế vừa đặt chuyển sang màu đỏ (occupied).
        return RedirectToAction("Index", new { movieId = model.MovieId, showtimeId = model.ShowtimeId });
    }

    public IActionResult Success()
    {
        return View();
    }
}
