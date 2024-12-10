using System;
using System.Collections.Generic;

namespace SmartBus.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int? BusId { get; set; }

    public int? RouteId { get; set; }

    public DateTime DepartureTime { get; set; }

    public DateTime ArrivalTime { get; set; }

    public decimal Fare { get; set; }

    public int AvailableSeats { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Bus? Bus { get; set; }

    public virtual Route? Route { get; set; }
}
