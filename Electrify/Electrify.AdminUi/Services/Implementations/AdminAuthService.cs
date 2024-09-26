using Electrify.Models.Models;
using Microsoft.AspNetCore.Identity;

namespace Electrify.AdminUi.Services.Implementations;

internal class AdminAuthService : IAdminAuthService
{
    private readonly PasswordHasher<Admin> hasher = new();
    public bool VerifyPassword(Admin admin, string plainTextPassword)
    {
        return hasher.VerifyHashedPassword(admin, admin.PasswordHash, plainTextPassword) != PasswordVerificationResult.Failed;
    }
}
