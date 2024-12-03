using Electrify.Models;
using Electrify.Server.Services.Abstraction;

namespace Electrify.Server.Database;

public class DatabaseSeeder(
    IAdminService adminService,
    ElectrifyDbContext dbContext,
    TimeProvider timeProvider)
    : IDatabaseSeeder
{
    private static readonly Random Random = new();

    private readonly Dictionary<string, string> _clientIds = new()
    {
        { "4b34de2e-c340-4aec-84bf-636e7a388410", "7fb094b0-27d0-4655-921b-f07211c68b9c" },
        { "e774ebc0-ea2c-4882-900d-bca4c37e535b", "6d9029fa-d22d-4507-84ec-090ad2bb84dd" },
        { "6842a005-1533-4abf-9abe-9a02b4ab304d", "852409aa-d892-4c25-b36b-1049bd7f2bd2" },
        { "23a487cf-9cb0-48a5-9a36-99db82192799", "64424f33-ac5e-4216-9b04-9f68bf33dfe0" },
        { "ac1516a8-1c1d-417e-bcf9-95d308a65c47", "b2bcc781-5a9e-4b00-8b9b-c6ed29ba527b" },
        { "e3f6fd12-fe66-4281-8709-1844703249ee", "5b1ae90e-d9e6-4536-9f5c-7a48ed8e5a90" },
        { "3c7c1e8e-4ccd-4b27-ae27-636d53376c59", "c591c609-0539-49b4-8133-596f8d707cb7" },
        { "83a3f16d-e0af-4e6e-a98a-62468625a0c3", "60d38667-52da-443f-a714-a7c4ca991013" },
        { "7d5758fb-15b0-4645-b682-8fdd64bdf6fe", "be641bab-727c-4c9c-8d30-0d7892291afc" },
        { "99a1d8fa-ea52-4447-a447-98b0e632cefe", "0ed67bdf-d4b0-46c3-8418-4e3aac146aad" },
        { "7fe1e7f6-b679-40a8-8bd8-d1c21c58baf4", "bba412a2-e6b7-439a-a238-e8f054eebe33" },
        { "3812f147-f064-4dd5-9584-f9397cddbd56", "922cb199-8df6-4c42-b9f0-850316b73f33" },
    };

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

        await dbContext.Clients.AddRangeAsync(_clientIds.Select(c => new Client
        {
            Id = Guid.Parse(c.Key),
            UserId = Guid.Parse(c.Value)
        }));

        await dbContext.SaveChangesAsync();
    }

    public async Task SeedDefaultReadings()
    {
        const int readingsCount = 20;
        var startTime = timeProvider.GetLocalNow().AddSeconds(-5 * (readingsCount + 1));

        var usage = 10.0;

        for (var i = 0; i < readingsCount; i++)
        {
            var dateTime = startTime.AddSeconds(5 * i);
            usage += 0.05 + Random.NextDouble() * 0.01;
            var tariff = 1 + Random.NextDouble();

            var clients = _clientIds.Select(c => new Client
            {
                Id = Guid.Parse(c.Key),
                UserId = Guid.Parse(c.Value)
            }).Take(6);
            
            foreach (var client in clients)
            {
                await dbContext.Readings.AddAsync(new Reading
                {
                    ClientId = client.Id,
                    DateTime = dateTime.DateTime,
                    EnergyUsage = usage,
                    Tariff = tariff
                });
            }
        }

        await dbContext.SaveChangesAsync();
    }
}