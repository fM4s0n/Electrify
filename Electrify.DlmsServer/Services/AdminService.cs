using Electrify.DlmsServer.Services.Abstraction;
using Electrify.Models.Models;
using Microsoft.AspNetCore.Identity;

namespace Electrify.DlmsServer.Services
{
    public class AdminService : IAdminService
    {
        private readonly PasswordHasher<Admin> _passwordHasher = new();

        public Admin CreateAdmin(string name, string email, string plainTextPassword)
        {
            Admin admin = new()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
            };

            // Hash the password
            admin.PasswordHash = _passwordHasher.HashPassword(admin, plainTextPassword);

            return admin;
        }

        public bool VerifyPassword(Admin admin, string plainTextPassword)
        {
            return _passwordHasher.VerifyHashedPassword(admin, admin.PasswordHash, plainTextPassword) != PasswordVerificationResult.Failed;
        }
    }
}
