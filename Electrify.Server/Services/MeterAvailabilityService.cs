using System.Diagnostics;
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
    private readonly List<CoolTimer> _timers = [];
    
    public override Task<AvailabilityResponse> Register(AvailabilityRequest request, ServerCallContext context)
    {
        var options = serviceProvider.GetRequiredService<IOptions<DlmsClientOptions>>().Value;
        var traceLevel = serviceProvider.GetRequiredService<IOptions<ObservabilityOptions>>().Value.TraceLevel;
        var logger = serviceProvider.GetRequiredService<ILogger<DlmsClient>>();
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

        var client = new DlmsClient(logger, media, reader, registers);

        _clients[request.Port] = client;
        
        var timer = new CoolTimer(delegate
        {
            logger.LogInformation("Energy Reading: {kWh}", _clients[request.Port].ReadEnergy());
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

class CoolTimer : IDisposable, IAsyncDisposable
{
    private readonly Timer _timer;
    private readonly Random _random = new();
    private readonly int _startRange;
    private readonly int _endRange;

    public CoolTimer(Action callback, int startRange, int endRange)
    {
        _startRange = startRange;
        _endRange = endRange;
        _timer = new Timer(_ => Callback(callback), null, 0, _random.Next(_startRange, _endRange));
    }

    private void Callback(Action callback)
    {
        callback.Invoke();
        _timer.Change(0, _random.Next(_startRange, _endRange));
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