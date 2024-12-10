using System;
using System.Collections.Generic;

namespace SmartBus.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? UserId { get; set; }

    public int? ScheduleId { get; set; }

    public DateTime? BookingDate { get; set; }

    public int NumberOfSeats { get; set; }

    public decimal TotalFare { get; set; }

    public string? Status { get; set; }

    public string? PaymentStatus { get; set; }

    public virtual ICollection<Cancellation> Cancellations { get; set; } = new List<Cancellation>();

    public virtual Schedule? Schedule { get; set; }

    public virtual User? User { get; set; }
}
