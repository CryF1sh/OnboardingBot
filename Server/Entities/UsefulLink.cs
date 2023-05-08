using System;
using System.Collections.Generic;

namespace Server.Entities;

public partial class UsefulLink
{
    public int Id { get; set; }

    public string Link { get; set; } = null!;

    public string? Description { get; set; }
}
