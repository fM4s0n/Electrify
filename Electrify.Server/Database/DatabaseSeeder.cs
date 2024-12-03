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
                UserId = Guid.Parse("7fb094b0-27d0-4655-921b-f07211c68b9c")
            },
            new Client
            {
                Id = Guid.Parse("e774ebc0-ea2c-4882-900d-bca4c37e535b"),
                UserId = Guid.Parse("6d9029fa-d22d-4507-84ec-090ad2bb84dd")
            },
            new Client
            {
                Id = Guid.Parse("6842a005-1533-4abf-9abe-9a02b4ab304d"),
                UserId = Guid.Parse("852409aa-d892-4c25-b36b-1049bd7f2bd2")
            },
            new Client
            {
                Id = Guid.Parse("23a487cf-9cb0-48a5-9a36-99db82192799"),
                UserId = Guid.Parse("64424f33-ac5e-4216-9b04-9f68bf33dfe0")
            },
            new Client
            {
                Id = Guid.Parse("ac1516a8-1c1d-417e-bcf9-95d308a65c47"),
                UserId = Guid.Parse("b2bcc781-5a9e-4b00-8b9b-c6ed29ba527b")
            },
            new Client
            {
                Id = Guid.Parse("e3f6fd12-fe66-4281-8709-1844703249ee"),
                UserId = Guid.Parse("5b1ae90e-d9e6-4536-9f5c-7a48ed8e5a90")
            },
            new Client
            {
                Id = Guid.Parse("3c7c1e8e-4ccd-4b27-ae27-636d53376c59"),
                UserId = Guid.Parse("c591c609-0539-49b4-8133-596f8d707cb7")
            },
            new Client
            {
                Id = Guid.Parse("83a3f16d-e0af-4e6e-a98a-62468625a0c3"),
                UserId = Guid.Parse("60d38667-52da-443f-a714-a7c4ca991013")
            },
            new Client
            {
                Id = Guid.Parse("7d5758fb-15b0-4645-b682-8fdd64bdf6fe"),
                UserId = Guid.Parse("be641bab-727c-4c9c-8d30-0d7892291afc")
            },
            new Client
            {
                Id = Guid.Parse("99a1d8fa-ea52-4447-a447-98b0e632cefe"),
                UserId = Guid.Parse("0ed67bdf-d4b0-46c3-8418-4e3aac146aad")
            },
            new Client
            {
                Id = Guid.Parse("7fe1e7f6-b679-40a8-8bd8-d1c21c58baf4"),
                UserId = Guid.Parse("bba412a2-e6b7-439a-a238-e8f054eebe33")
            },
            new Client
            {
                Id = Guid.Parse("3812f147-f064-4dd5-9584-f9397cddbd56"),
                UserId = Guid.Parse("922cb199-8df6-4c42-b9f0-850316b73f33")
            }
        );

        await dbContext.SaveChangesAsync();
    }

    public async Task SeedDefaultReadings()
    {
        Guid[] clients =
        [
            Guid.Parse("4b34de2e-c340-4aec-84bf-636e7a388410"),
            Guid.Parse("e774ebc0-ea2c-4882-900d-bca4c37e535b"),
            Guid.Parse("6842a005-1533-4abf-9abe-9a02b4ab304d"),
            Guid.Parse("23a487cf-9cb0-48a5-9a36-99db82192799"),
            Guid.Parse("ac1516a8-1c1d-417e-bcf9-95d308a65c47"),
            Guid.Parse("e3f6fd12-fe66-4281-8709-1844703249ee")
        ];

        const int readingsCount = 20;
        var startTime = timeProvider.GetLocalNow().AddSeconds(-5 * (readingsCount + 1));

        var usage = 0.01;

        for (var i = 0; i < readingsCount; i++)
        {
            var dateTime = startTime.AddSeconds(5 * i);
            usage += 0.05 + Random.NextDouble() * 0.01;
            var tariff = Random.NextDouble();

            foreach (var client in clients)
            {
                await dbContext.Readings.AddAsync(new Reading
                {
                    ClientId = client,
                    DateTime = dateTime.DateTime,
                    EnergyUsage = usage,
                    Tariff = tariff
                });
            }
        }

        await dbContext.SaveChangesAsync();
    }
}