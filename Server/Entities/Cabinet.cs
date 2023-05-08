using System;
using System.Collections.Generic;

namespace Server.Entities;

public partial class Cabinet
{
    public int Id { get; set; }

    public int Number { get; set; }

    public string? Name { get; set; }

    public int? EmployeeId { get; set; }

    public int? FloorLayoutId { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual FloorLayout? FloorLayout { get; set; }
}
