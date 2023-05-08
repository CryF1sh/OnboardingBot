using System;
using System.Collections.Generic;

namespace Server.Entities;

public partial class EmployeeDirection
{
    public int EmployeeId { get; set; }

    public int DirectionId { get; set; }

    public virtual Direction Direction { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;
}
