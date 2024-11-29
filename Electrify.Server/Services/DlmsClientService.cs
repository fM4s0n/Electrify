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
    private readonly Dictionary<Guid, IDlmsClient> _clients = [];
    private readonly Dictionary<Guid,RandomTaskTimer> _timers = [];

    public void AddClient(int port, Guid clientId, IDlmsClient client)
    {
        _clients[clientId] = client;
        
        var timer = new RandomTaskTimer(timeProvider, delegate
        {
            using var scope = serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<ElectrifyDbContext>();

            var lastReading = database.GetLastReading(clientId) ?? timeProvider.GetLocalNow().AddMinutes(-1).DateTime;
            var readings = _clients[clientId].ReadEnergyProfile(lastReading).ToList();

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
        
        _timers[clientId] = timer;
    }

    public IEnumerable<IDlmsClient> GetClients()
    {
        return _clients.Values;
    }

    public void TryRemoveClient(Guid clientId)
    {
        _timers.TryGetValue(clientId, out var timer); 
        timer?.Dispose();
        _timers.Remove(clientId);
        _clients.Remove(clientId);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var timer in _timers.Values)
        {
            await timer.DisposeAsync();
        }
    }
}