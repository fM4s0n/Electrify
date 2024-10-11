using Electrify.Models.Models;
using Electrify.Server.Services.Abstraction;
using Microsoft.AspNetCore.Identity;

namespace Electrify.DlmsServer.Services
{
    public class AdminService(PasswordHasher<Admin> passwordHasher) : IAdminService
    {
        public Admin CreateAdmin(string name, string email, string plainTextPassword)
        {
            Admin admin = new()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                PasswordHash = string.Empty,
            };

            // Hash the password
            admin.PasswordHash = passwordHasher.HashPassword(admin, plainTextPassword);

            return admin;
        }

        public bool VerifyPassword(Admin admin, string plainTextPassword)
        {
            return passwordHasher.VerifyHashedPassword(admin, admin.PasswordHash, plainTextPassword) != PasswordVerificationResult.Failed;
        }
    }
}
