using CineTicket.Models;

public class Ticket
{
    public int Id { get; set; }

    // Quan hệ với Showtime
    public int ShowtimeId { get; set; }
    public Showtime Showtimes { get; set; }

    // Tự động lấy Movie từ Showtime nếu cần (không nên dùng Include trực tiếp)

    public string SeatNumber { get; set; }
    public decimal Price { get; set; }

    // Nếu có người dùng
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}
