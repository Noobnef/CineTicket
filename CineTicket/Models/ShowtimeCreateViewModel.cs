using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CineTicket.Models {
    public class ShowtimeCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn phim.")]
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phòng.")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian chiếu.")]
        public DateTime StartTime { get; set; }

        // Dùng để render dropdown
        public List<SelectListItem> Movies { get; set; }
        public List<SelectListItem> Rooms { get; set; }
    }
}
