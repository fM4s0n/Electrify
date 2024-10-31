using Electrify.Models.Models;
using Electrify.Server.Database;
using Electrify.Server.Services.Abstraction;
using Microsoft.AspNetCore.Identity;

namespace Electrify.Server.Services
{
    public class AdminService(ElectrifyDbContext dbContext) : IAdminService 
    {
        private readonly ElectrifyDbContext _dbContext = dbContext;
        private static readonly PasswordHasher<Admin> _passwordHasher = new();              

        public void CreateAdmin(string name, string email, string plainTextPassword)
        {
            Admin admin = new()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                PasswordHash = string.Empty,
            };

            // Hash the password
            admin.PasswordHash = _passwordHasher.HashPassword(admin, plainTextPassword);

            InsertAdmin(admin);
        }

        public bool VerifyPassword(Admin admin, string plainTextPassword)
        {
            return _passwordHasher.VerifyHashedPassword(admin, admin.PasswordHash, plainTextPassword) != PasswordVerificationResult.Failed;
        }

        public Guid GenerateAccessToken()
        {
            return Guid.NewGuid();
        }

        private void InsertAdmin(Admin admin)
        {
            _dbContext.Admins.Add(admin);
            _dbContext.SaveChanges();
        }

        public void UpdateAccessToken(Admin admin, Guid? token)
        {
            admin.AccessToken = token;
            _dbContext.Admins.Update(admin);
            _dbContext.SaveChanges();
        }

        public Admin? GetAdminByEmail(string email)
        {
            return _dbContext.Admins.FirstOrDefault(a => a.Email == email);
        }
    }
}
