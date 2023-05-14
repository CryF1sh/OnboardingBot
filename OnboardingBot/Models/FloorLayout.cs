using System;
using System.Collections.Generic;

namespace Server.Entities;

public partial class FloorLayout
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? PhotoLink { get; set; }

    //public virtual ICollection<Cabinet> Cabinets { get; set; } = new List<Cabinet>();
}
