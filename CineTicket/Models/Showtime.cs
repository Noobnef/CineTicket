using System;
using System.ComponentModel.DataAnnotations;

namespace CineTicket.Models
{
    public class Showtime
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian chiếu.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phim.")]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phòng.")]
        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}
