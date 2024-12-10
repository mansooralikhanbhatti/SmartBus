using System.ComponentModel.DataAnnotations;

namespace SmartBus.DTOs
{
    public class BookingDto
    {
        [Required]
        public int ScheduleId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [Range(1, 10)]
        public int NumberOfSeats { get; set; }
    }
}
