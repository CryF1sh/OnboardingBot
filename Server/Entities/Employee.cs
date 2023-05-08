using System;
using System.Collections.Generic;
using Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Server.Entities;

public partial class Employee
{
    public int Id { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string MiddleName { get; set; } = null!;

    public string Position { get; set; } = null!;

    public string? Description { get; set; }

    public string? PhotoLink { get; set; }

    public string? Email { get; set; }

    public string? Telephone { get; set; }

    public string? VkLink { get; set; }

    public string? TelegramLink { get; set; }

    public virtual ICollection<Cabinet> Cabinets { get; set; } = new List<Cabinet>();
}