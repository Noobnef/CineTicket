using CineTicket.Models;


namespace CineTicket.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public DateTime BookingTime { get; set; } = DateTime.Now;


        // Quan hệ với Showtime
        public int ShowtimeId { get; set; }
        public Showtime Showtimes { get; set; }

        // Tự động lấy Movie từ Showtime nếu cần (không nên dùng Include trực tiếp)

        public string SeatNumber { get; set; }
        public int Price { get; set; }

        // Nếu có người dùng
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}