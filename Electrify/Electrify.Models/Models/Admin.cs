using Microsoft.AspNetCore.Identity;

namespace Electrify.Models.Models;

/// <summary>
/// Model for the Electrify Admin.
/// </summary>
public class Admin
{
    // TODO: MAKE THIS A FACTORY
    private static readonly PasswordHasher<Admin> hasher = new();

    public Admin() { }

    public Admin(string name, string email, string plainTextPassword)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = hasher.HashPassword(this, plainTextPassword);
    }

    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }

    /// <summary>
    /// Valuidates the password passed in from the user.
    /// </summary>
    /// <param name="plainTextPassword">The password provided by the user.</param>
    /// <returns>True if the result is Success or SuccessRehashNeeded, otherwise false.</returns>
    public bool VerifyPassword(string plainTextPassword)
    {
        return hasher.VerifyHashedPassword(this, PasswordHash, plainTextPassword) != PasswordVerificationResult.Failed;
    }
}
