using Electrify.Dlms.Client.Abstraction;
using Electrify.Models;
using Electrify.Server.Database;
using Electrify.Server.Services.Abstraction;

namespace Electrify.Server.Services;

public sealed class DlmsClientService(
    TimeProvider timeProvider,
    ILogger<DlmsClientService> logger,
    IServiceProvider serviceProvider)
    : IDlmsClientService
{
    private readonly Dictionary<int, IDlmsClient> _clients = [];
    private readonly List<RandomTaskTimer> _timers = [];

    public IDlmsClient? TryGetClient(int port)
    {
        _clients.TryGetValue(port, out var client);
        return client;
    }

    public void AddClient(int port, Guid clientId, IDlmsClient client)
    {
        _clients[port] = client;
        
        var timer = new RandomTaskTimer(timeProvider, delegate
        {
            using var scope = serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<ElectrifyDbContext>();

            var lastReading = database.GetLastReading(clientId) ?? timeProvider.GetLocalNow().AddMinutes(-1).DateTime;
            var readings = _clients[port].ReadEnergyProfile(lastReading).ToList();

            for (var i = 0; i < readings.Count; i++)
            {
                Reading reading = readings[i];

                if (database.Readings
                        .Any(dbReading => dbReading.ClientId == clientId && dbReading.DateTime == reading.DateTime) == false)
                {
                    database.Readings.Add(reading);
                }

                database.SaveChanges();
                
                logger.LogInformation("Energy Reading {Index}: ClientId: {ClientId} DateTime: {DateTime} EnergyUsage: {EnergyUsage} Tariff: {EnergyTariff}",
                    i, clientId, reading.DateTime, reading.EnergyUsage, reading.Tariff);
            }
        }, 15, 60);
        
        _timers.Add(timer);
    }

    public IEnumerable<IDlmsClient> GetClients()
    {
        return _clients.Values;
    }
    
    public async ValueTask DisposeAsync()
    {
        foreach (var timer in _timers)
        {
            await timer.DisposeAsync();
        }
    }
}