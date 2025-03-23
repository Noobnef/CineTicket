using CineTicket.Models;

namespace CineTicket.Models
{
    public class Showtimes
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
