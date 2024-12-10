using System.ComponentModel.DataAnnotations;

namespace SmartBus.DTOs
{
    public class CancellationDto
    {
        [Required]
        public int BookingId { get; set; }
        [Required]
        [MinLength(10)]
        public string CancellationReason { get; set; }
    }
}
