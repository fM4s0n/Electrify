using Electrify.Server.Services.Abstraction;

namespace Electrify.Server.Database;

public class DatabaseSeeder(IAdminService adminService, ElectrifyDbContext dbContext) : IDatabaseSeeder
{
    /// <summary>
    /// Seeds the default admin account for AdminUI.
    /// </summary>
    public void SeedDefaultAdmin()
    {
        if (dbContext.Admins.Any(a => a.Email == "admin@electrify.com"))
        {
            return;
        }

        adminService.CreateAdmin("Administrator", "admin@electrify.com", "Password");
    } 
}
