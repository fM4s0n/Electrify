using Electrify.Models;
using Electrify.Server.Services.Abstraction;

namespace Electrify.Server.Database;

public class DatabaseSeeder(IAdminService adminService, ElectrifyDbContext dbContext) : IDatabaseSeeder
{
    /// <summary>
    /// Seeds the default admin account for AdminUI.
    /// </summary>
    public void SeedDefaultAdmin()
    {

        dbContext.Clients.Add(new Client
        {
            Id = Guid.Parse("2be2526d-feac-4859-a2b2-3d6ea699da5e"),
            UserId = Guid.Parse("2be2526d-feac-4859-a2b2-3d6ea699da5e")
        });
        
        dbContext.Clients.Add(new Client
        {
            Id = Guid.Parse("2be2526d-feac-4859-a2b2-3d6ea699da5f"),
            UserId = Guid.Parse("2be2526d-feac-4859-a2b2-3d6ea699da5f")
        });

        dbContext.SaveChanges();
        
        if (dbContext.Admins.Any(a => a.Email == "admin@electrify.com"))
        {
            return;
        }

        adminService.CreateAdmin("Administrator", "admin@electrify.com", "Password");
    } 
}
