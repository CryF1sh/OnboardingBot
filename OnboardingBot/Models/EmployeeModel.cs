public partial class Employee
{
    public int Id { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string MiddleName { get; set; } = null!;

    public string FullName => $"{LastName} {FirstName} {MiddleName}";

    public string Position { get; set; } = null!;

    public string? Description { get; set; }

    public string? PhotoLink { get; set; }

    public string? Email { get; set; }

    public string? Telephone { get; set; }

    public string? VkLink { get; set; }

    public string? TelegramLink { get; set; }
}