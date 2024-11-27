using Electrify.Models;
using Electrify.Server.Services.Abstraction;

namespace Electrify.Server.Database;

public class DatabaseSeeder(IAdminService adminService, ElectrifyDbContext dbContext) : IDatabaseSeeder
{
    /// <summary>
    /// Seeds the default admin account for AdminUI.
    /// </summary>
    public async Task SeedDefaultAdmin()
    {
        if (dbContext.Admins.Any(a => a.Email == "admin@electrify.com"))
        {
            return;
        }
        
        await adminService.CreateAdmin("Administrator", "admin@electrify.com", "Password");
    }

    public async Task SeedDefaultClientId()
    {
        if (dbContext.Clients.Any(c => c.Id == Guid.Parse("4b34de2e-c340-4aec-84bf-636e7a388410")))
        {
            return;
        }
        
        await dbContext.Clients.AddRangeAsync(new Client
        {
            Id = Guid.Parse("4b34de2e-c340-4aec-84bf-636e7a388410"),
            UserId = Guid.Parse("4b34de2e-c340-4aec-84bf-636e7a388410")
        },
        new Client
        {
            Id = Guid.Parse("e774ebc0-ea2c-4882-900d-bca4c37e535b"),
            UserId = Guid.Parse("e774ebc0-ea2c-4882-900d-bca4c37e535b")
        });

        await dbContext.SaveChangesAsync();
    }
}
