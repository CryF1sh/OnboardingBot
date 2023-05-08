using System;
using System.Collections.Generic;

namespace Server.Entities;

public partial class Direction
{
    public int Id { get; set; }

    public string NameDirection { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
