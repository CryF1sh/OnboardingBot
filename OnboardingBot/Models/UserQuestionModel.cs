public partial class UserQuestion
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Question { get; set; } = null!;

    public TimeOnly DateTimeQuestion { get; set; }

    public string? Answer { get; set; }

    public TimeOnly? DateTimeAnswer { get; set; }

}
