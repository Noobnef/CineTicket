namespace CineTicket.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public int SeatNumber { get; set; }         // Ghế số (1 - N)
        public string Row { get; set; }             // Hàng ghế: A, B, C...

        public int RoomId { get; set; }
        public Room Room { get; set; }
    }

}
