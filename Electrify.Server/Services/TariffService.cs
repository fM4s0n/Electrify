using Electrify.Models;
using Electrify.Server.Database;
using Electrify.Server.Options;
using Electrify.Server.Services.Abstraction;
using Microsoft.Extensions.Options;

namespace Electrify.Server.Services;

public class TariffService(
    IOctopusService octopusService,
    TimeProvider timeProvider,
    IServiceProvider serviceProvider,
    ILogger<TariffService> logger,
    IOptions<TariffOptions> options,
    IDlmsClientService dlmsClientService)
    : BackgroundService
{
    private Timer? _timer;
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var period = options.Value.TariffUpdateInterval;
        var localTime = timeProvider.GetLocalNow();

        var remainder = new TimeSpan(localTime.Ticks % period.Ticks);
        var dueTime = remainder == TimeSpan.Zero ? remainder : period - remainder;
        
        // ReSharper disable once AsyncVoidLambda
        _timer = new Timer(async _ => await UpdateTariff(), null, dueTime, period);
        return Task.CompletedTask;
    }
    
    private async Task UpdateTariff()
    {
        using var scope = serviceProvider.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<ElectrifyDbContext>();
        
        var localNow = timeProvider.GetLocalNow();
        
        var currentTariff = database.Tariffs.FirstOrDefault(
            t => t.DateTime.Date == localNow.Date && t.DateTime.Hour == localNow.Hour);
        
        if (currentTariff is null)
        {
            var daysPrices = await octopusService.GetDailyPrices(localNow);
        
            if (daysPrices is null)
            {
                currentTariff = database.Tariffs.MaxBy(t => t.DateTime);
            }
            else
            {
                foreach (var (hour, price) in daysPrices)
                {
                    var dateTime = new DateTime(localNow.Year, localNow.Month, localNow.Day, hour, 0, 0);
                    
                    await database.Tariffs.AddAsync(new Tariff
                    {
                        DateTime = dateTime,
                        Price = price
                    });
                }
                    
                await database.SaveChangesAsync();
        
                currentTariff = database.Tariffs.FirstOrDefault(
                    t => t.DateTime.Date == localNow.Date && t.DateTime.Hour == localNow.Hour);
            }
        }
        
        if (currentTariff is null)
        {
            logger.LogError("No tariff was found in the database or the octopus API for the current time");
            return;
        }
    
        foreach (var dlmsClient in dlmsClientService.GetClients())
        {
            dlmsClient.WriteTariff(currentTariff.Price);
        }
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}