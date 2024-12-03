namespace Electrify.Server.Services.Abstraction;

public interface IDatabaseSeeder
{
    Task SeedDefaultAdmin();
    Task SeedDefaultClientId();
    Task SeedDefaultReadings();
}
