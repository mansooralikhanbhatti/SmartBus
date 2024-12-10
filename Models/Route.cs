using System;
using System.Collections.Generic;

namespace SmartBus.Models;

public partial class Route
{
    public int RouteId { get; set; }

    public string Source { get; set; } = null!;

    public string Destination { get; set; } = null!;

    public decimal? Distance { get; set; }

    public TimeOnly? Duration { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
