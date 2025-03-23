using CineTicket.Models;

namespace CineTicket.Models
{
    public class Tickets
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; }
        public int ShowtimeId { get; set; }
        public Showtimes Showtimes { get; set; }
        public decimal Price { get; set; }
        public string SeatNumber { get; set; }

    }
}
