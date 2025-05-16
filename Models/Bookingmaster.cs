using System;
using System.Collections.Generic;

namespace SICProject.Models;

public partial class Bookingmaster
{
    public int BookingId { get; set; }

    public int? InstrumentId { get; set; }

    public int? StudentId { get; set; }

    public string? Remarks { get; set; }

    public DateOnly? BookingDate { get; set; }

    public TimeOnly? SlotStart { get; set; }

    public TimeOnly? SlotEnd { get; set; }
}
