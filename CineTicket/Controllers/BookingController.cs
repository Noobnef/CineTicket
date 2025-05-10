using Microsoft.AspNetCore.Mvc;
using CineTicket.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using CineTicket.Repositories;
using PdfSharpCore.Drawing.Layout;

public class BookingController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGmailSender _gmailSender;

    public BookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IGmailSender gmailSender)
    {
        _context = context;
        _userManager = userManager;
        _gmailSender = gmailSender;
    }

    public IActionResult Index(int movieId, int? showtimeId)
    {
        var movie = _context.Movies.FirstOrDefault(m => m.Id == movieId);
        if (movie == null)
            return NotFound("Không tìm thấy phim.");

        if (!showtimeId.HasValue)
        {
            var showtimes = _context.Showtimes
                .Where(s => s.MovieId == movieId && s.StartTime > DateTime.Now)
                .OrderBy(s => s.StartTime)
                .ToList();

            if (!showtimes.Any())
                return BadRequest("Phim này chưa có suất chiếu.");

            return View("ChooseShowtime", new ChooseShowtimeViewModel
            {
                MovieId = movie.Id,
                MovieTitle = movie.Title,
                Showtimes = showtimes
            });
        }

        var selectedShowtime = _context.Showtimes
            .Include(s => s.Room)
            .FirstOrDefault(s => s.Id == showtimeId.Value && s.MovieId == movieId);

        if (selectedShowtime == null)
            return BadRequest("Suất chiếu không hợp lệ.");

        var bookedSeats = _context.Tickets
            .Where(t => t.ShowtimeId == selectedShowtime.Id)
            .Select(t => t.SeatNumber)
            .ToList();

        var booking = new BookingViewModel
        {
            MovieId = movie.Id,
            MovieTitle = movie.Title,
            TicketPrice = selectedShowtime.Room.TicketPrice,
            ShowtimeId = selectedShowtime.Id,
            AlreadyBookedSeats = bookedSeats
        };

        return View(booking);
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmBooking(BookingViewModel model)
    {
        if (string.IsNullOrEmpty(model.SeatNumbers))
        {
            return RedirectToAction("Index", new { movieId = model.MovieId, showtimeId = model.ShowtimeId });
        }

        var seats = model.SeatNumbers.Split(',');
        var userId = User.Identity.IsAuthenticated ? _userManager.GetUserId(User) : null;
        decimal totalPrice = model.TicketPrice * seats.Length;

        foreach (var seat in seats)
        {
            var ticket = new Ticket
            {
                ShowtimeId = model.ShowtimeId,
                SeatNumber = seat,
                Price = model.TicketPrice,
                BookingTime = DateTime.Now,
                UserId = userId
            };
            _context.Tickets.Add(ticket);
        }

        var history = new BookingHistory
        {
            UserId = userId,
            ShowtimeId = model.ShowtimeId,
            SeatNumbers = model.SeatNumbers,
            TotalAmount = totalPrice,
            BookingDate = DateTime.Now
        };
        _context.BookingHistories.Add(history);

        await _context.SaveChangesAsync();

        var movie = await _context.Movies.FindAsync(model.MovieId);
        var pdfBytes = GenerateTicketPdf(movie.Title, model.SeatNumbers, totalPrice);

        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            await _gmailSender.SendEmailWithAttachmentAsync(user.Email, "Vé xem phim CineTicket",
                $"Cảm ơn bạn đã đặt vé cho phim: {movie.Title}. " +
                $"Ghế: {model.SeatNumbers}. " +
                $"Tổng tiền: {totalPrice:N0} VND.",
                pdfBytes, "ve-phim.pdf");
        }

        return RedirectToAction("Success");
    }

    public IActionResult Success()
    {
        return View();
    }

    private byte[] GenerateTicketPdf(string movie, string seats, decimal amount)
    {
        using var ms = new MemoryStream();
        var doc = new PdfDocument();
        var page = doc.AddPage();
        var gfx = XGraphics.FromPdfPage(page);

        var font = new XFont("Arial", 14);
        var brush = XBrushes.Black;

        string content = $"🎬 Vé Xem Phim\n" +
                         $"Phim: {movie}\n" +
                         $"Ghế: {seats}\n" +
                         $"Tổng tiền: {amount:N0} VND\n" +
                         $"Ngày đặt: {DateTime.Now:dd/MM/yyyy HH:mm}";

        var rect = new XRect(40, 40, page.Width - 80, page.Height - 80);
        gfx.DrawString(content, font, brush, rect, XStringFormats.TopLeft);

        doc.Save(ms);
        return ms.ToArray();
    }

}
