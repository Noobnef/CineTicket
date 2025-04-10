namespace CineTicket.Models
{
    public class BookingHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ShowtimeId { get; set; }
        public string SeatNumbers { get; set; } // "A1,B2,C3"
        public decimal TotalAmount { get; set; }
        public DateTime BookingDate { get; set; }
    }

}
