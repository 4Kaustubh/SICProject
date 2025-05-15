using System;
using System.Collections.Generic;

namespace SICProject.Models;

public partial class Holidaymaster
{
    public int HolidayId { get; set; }

    public DateOnly? HolidayDate { get; set; }

    public string? Reson { get; set; }
}
