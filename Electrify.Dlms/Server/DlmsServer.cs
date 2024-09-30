using System.Diagnostics;
using Electrify.Dlms.Options;
using Electrify.Dlms.Server.Abstraction;
using Gurux.DLMS.Enums;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Objects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Electrify.Dlms.Server;

public sealed class DlmsServer : IDlmsServer
{
    private readonly GXDLMSAssociationLogicalName _association;
    private readonly GXDLMSBase _server;
    private readonly ILogger<DlmsServer> _logger;
    private readonly CancellationTokenSource _cts = new();
    
    public DlmsServer(
        GXDLMSAssociationLogicalName association,
        GXDLMSBase server,
        Action<DlmsServer> configure,
        IOptions<DlmsServerOptions> options,
        TraceLevel traceLevel,
        ILogger<DlmsServer> logger)
    {
        _association = association;
        _server = server;
        _logger = logger;

        configure.Invoke(this);
        
        _server.Initialize(options.Value.Port, traceLevel);

        _ = RunAsync(_cts.Token);
    }

    public void AddRegister(GXDLMSRegister register)
    {
        _association.SetAccess3(register, 3, AccessMode3.Read);
        _association.SetAccess3(register, 2, AccessMode3.Read);
        
        _server.Items.Add(register);
    }

    public double GetEnergy()
    {
        foreach (var dlmsObject in _server.Items)
        {
            // TODO maybe this string should be done via IOptions
            if (dlmsObject is GXDLMSRegister { LogicalName: "1.1.1.8.0.255", Value: int value })
            {
                return value;
            }
        }

        return 0;
    }
    
    public void SetEnergy(int energyValue)
    {
        foreach (GXDLMSObject? dlmsObject in _server.Items)
        {
            // TODO maybe this string should be done via IOptions
            if (dlmsObject is GXDLMSRegister { LogicalName: "1.1.1.8.0.255" } register)
            {
                register.Value = energyValue;
            }
        }
    }

    private Task RunAsync(CancellationToken cancellationToken)
    {
        var resetEvent = new AutoResetEvent(false);

        while (!cancellationToken.IsCancellationRequested)
        {
            var waitTime = _server.Run(resetEvent);
            _logger.LogInformation("Waiting {TimeSpan} before next execution", TimeSpan.FromSeconds(waitTime));
            resetEvent.WaitOne(waitTime * 1000);
        }

        return Task.CompletedTask;
    }
    public void Dispose()
    {
        _cts.Cancel();
    }
}