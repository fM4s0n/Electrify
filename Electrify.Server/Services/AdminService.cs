using Electrify.Models;
using Electrify.Server.Database;
using Electrify.Server.Services.Abstraction;
using Microsoft.AspNetCore.Identity;

namespace Electrify.Server.Services;

public class AdminService(ElectrifyDbContext dbContext, PasswordHasher<Admin> passwordHasher) : IAdminService 
{
    public async Task CreateAdmin(string name, string email, string plainTextPassword)
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

        await InsertAdmin(admin);
    }

    public bool VerifyPassword(Admin admin, string plainTextPassword)
    {
        return passwordHasher.VerifyHashedPassword(admin, admin.PasswordHash, plainTextPassword) != PasswordVerificationResult.Failed;
    }

    public Guid GenerateAccessToken()
    {
        return Guid.NewGuid();
    }

    private async Task InsertAdmin(Admin admin)
    {
        await dbContext.Admins.AddAsync(admin);
        await dbContext.SaveChangesAsync();
    }

    public void UpdateAccessToken(Admin admin, Guid? token)
    {
        admin.AccessToken = token;
        dbContext.Admins.Update(admin);
        dbContext.SaveChanges();
    }

    public Admin? GetAdminByEmail(string email)
    {
        return dbContext.Admins.FirstOrDefault(a => a.Email == email);
    }
}