using System;
using System.Collections.Generic;

namespace Server.Entities;

public partial class User
{
    public int Id { get; set; }

    public int TelegramId { get; set; }

    public int DirectionId { get; set; }

    public virtual Direction Direction { get; set; } = null!;

    public virtual ICollection<UserQuestion> UserQuestions { get; set; } = new List<UserQuestion>();
}
