using Electrify.Dlms.Client;
using Electrify.Dlms.Client.Abstraction;
using Electrify.Dlms.Options;
using Electrify.DlmsServer;
using Electrify.Server.Options;
using Grpc.Core;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Secure;
using Gurux.Net;
using Microsoft.Extensions.Options;

namespace Electrify.Server.Services;

public class MeterAvailabilityService(ILogger<MeterAvailabilityService> logger, IServiceProvider serviceProvider) : MeterAvailability.MeterAvailabilityBase, IDisposable
{
    private readonly Dictionary<int, IDlmsClient> _clients = [];
    private readonly List<RandomTaskTimer> _timers = [];
    
    public override Task<AvailabilityResponse> Register(AvailabilityRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.ClientId, out var clientId))
        {
            // TODO use bad request instead or something, at least at problem details to response
            return Task.FromResult(new AvailabilityResponse
            {
                Success = false
            });
        }
        
        var options = serviceProvider.GetRequiredService<IOptions<DlmsClientOptions>>().Value;
        var traceLevel = serviceProvider.GetRequiredService<IOptions<ObservabilityOptions>>().Value.TraceLevel;
        var logger = serviceProvider.GetRequiredService<ILogger<DlmsClient>>();
        var timeProvider = serviceProvider.GetRequiredService<TimeProvider>();
        var media = new GXNet(options.Protocol, options.ServerHostname, request.Port);
        
        var secureClient = new GXDLMSSecureClient(
            options.UseLogicalNameReferencing,
            options.ClientAddress,
            options.ServerAddress,
            options.Authentication,
            request.Secret,
            options.InterfaceType);

        var reader = new GXDLMSReader(secureClient, media, traceLevel, options.InvocationCounter);

        var registers = options.LogicalNames
            .Replace(" ", "")
            .Split(",")
            .Select(register => new GXDLMSRegister(register));

        var client = new DlmsClient(clientId, logger, media, reader, registers, timeProvider);

        _clients[request.Port] = client;
        
        var timer = new RandomTaskTimer(timeProvider, delegate
        {
            var readings = _clients[request.Port].ReadEnergyProfile(timeProvider.GetUtcNow().AddDays(-1).DateTime).ToList();

            for (var i = 0; i < readings.Count; i++)
            {
                var reading = readings[i];
                logger.LogInformation("Energy Reading {Index}: ClientId: {ClientId} DateTime: {DateTime} EnergyUsage: {EnergyUsage} Tariff: {EnergyTariff}",
                    i, clientId, reading.DateTime, reading.EnergyUsage, reading.Tariff);
            }
        }, 15_000, 60_000);
        
        _timers.Add(timer);

        return Task.FromResult(new AvailabilityResponse
        {
            Success = true
        });
    }

    public void Dispose()
    {
        foreach (var timer in _timers)
        {
            timer.Dispose();
        }
        
        // TODO disconnect all the clients
        foreach (var client in _clients.Values)
        {
            
        }

        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// After executing the callback the timer changes its period to a value between
/// the StartRange and EndRange it was initialised with.
/// </summary>
internal class RandomTaskTimer : IDisposable, IAsyncDisposable
{
    private readonly ITimer _timer;
    private readonly Random _random = new();
    private readonly int _startRange;
    private readonly int _endRange;

    public RandomTaskTimer(TimeProvider timeProvider, Action callback, int startRange, int endRange)
    {
        _startRange = startRange;
        _endRange = endRange;

        _timer = timeProvider.CreateTimer(
            _ => Callback(callback), 
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(_random.Next(_startRange, _endRange)));
    }

    private void Callback(Action callback)
    {
        callback.Invoke();
        _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(_random.Next(_startRange, _endRange)));
    }
    public void Dispose()
    {
        _timer.Dispose();
    }
    public async ValueTask DisposeAsync()
    {
        await _timer.DisposeAsync();
    }
}