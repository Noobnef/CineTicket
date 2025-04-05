namespace CineTicket.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }           // Ví dụ: Phòng 1, Phòng VIP
        public int SeatCount { get; set; }

        public ICollection<Seat> Seats { get; set; }
        public ICollection<Showtime> Showtimes { get; set; }
    }

}
