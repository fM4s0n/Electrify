using Electrify.Server.Services.Abstraction;

namespace Electrify.Server.Database;

public class DatabaseSeeder(IAdminService adminService, ElectrifyDbContext dbContext) : IDatabaseSeeder
{
    private readonly IAdminService _adminService = adminService;
    private readonly ElectrifyDbContext _dbContext = dbContext;

    /// <summary>
    /// Seeds the default admin account for AdminUI.
    /// </summary>
    public void SeedDefaultAdmin()
    {
        if (_dbContext.Admins.Any(a => a.Email == "admin@electrify.com"))
        {
            return;
        }

        _adminService.CreateAdmin("Administrator", "admin@electrify.com", "Password");
    } 
}
