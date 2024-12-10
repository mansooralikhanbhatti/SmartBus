using System;
using System.Collections.Generic;

namespace SmartBus.Models;

public partial class Cancellation
{
    public int CancellationId { get; set; }

    public int? BookingId { get; set; }

    public string CancellationReason { get; set; } = null!;

    public DateTime? CancellationDate { get; set; }

    public decimal? RefundAmount { get; set; }

    public string? RefundStatus { get; set; }

    public virtual Booking? Booking { get; set; }
}
