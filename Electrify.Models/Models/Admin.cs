namespace Electrify.Models.Models;

/// <summary>
/// Model for the Electrify Admin.
/// </summary>
public sealed record Admin
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public Guid? AccessToken { get; set; }
}
