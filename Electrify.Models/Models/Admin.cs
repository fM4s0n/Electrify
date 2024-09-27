namespace Electrify.Models.Models;

/// <summary>
/// Model for the Electrify Admin.
/// </summary>
public class Admin
{
    public Admin() { }

    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
}
