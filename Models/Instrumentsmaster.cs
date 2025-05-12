using System;
using System.Collections.Generic;

namespace SICProject.Models;

public partial class Instrumentsmaster
{
    public int InstrumentsId { get; set; }

    public string InstrumentName { get; set; } = null!;

    public string Remarks { get; set; } = null!;
}
