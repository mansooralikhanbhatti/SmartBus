using System;
using System.Collections.Generic;

namespace SmartBus.Models;

public partial class Bus
{
    public int BusId { get; set; }

    public string BusNumber { get; set; } = null!;

    public int TotalSeats { get; set; }

    public string? BusType { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
