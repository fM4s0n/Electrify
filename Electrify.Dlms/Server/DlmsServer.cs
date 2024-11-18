using Electrify.Dlms.Constants;
using Electrify.Dlms.Options;
using Electrify.Dlms.Server.Abstraction;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Microsoft.Extensions.DependencyInjection;
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

    public DlmsServer(Action<DlmsServer, IServiceProvider> configure, IServiceProvider serviceProvider)
    {
        _association = serviceProvider.GetRequiredService<GXDLMSAssociationLogicalName>();
        _server = serviceProvider.GetRequiredService<GXDLMSBase>();
        _logger = serviceProvider.GetRequiredService<ILogger<DlmsServer>>();

        configure.Invoke(this, serviceProvider);
    }

    public void AddObject(GXDLMSObject dlmsObject, AccessMode3 valueAccessMode = AccessMode3.Read)
    {
        _association.SetAccess3(dlmsObject, 3, AccessMode3.Read);
        _association.SetAccess3(dlmsObject, 2, valueAccessMode);
        
        _server.Items.Add(dlmsObject);
    }
    
    public void SetEnergy(int energyValue)
    {
        foreach (GXDLMSObject? dlmsObject in _server.Items)
        {
            // TODO maybe this string should be done via IOptions
            if (dlmsObject is GXDLMSRegister { LogicalName: RegisterNames.EnergyUsage } register)
            {
                register.Value = energyValue;
            }
        }
    }

    public void Initialise(IOptions<DlmsServerOptions> options, Action onConnectedCallback, Action onDisconnectedCallback)
    {
        _server.OnConnectedCallback = onConnectedCallback;
        _server.OnDisconnectedCallback = onDisconnectedCallback;

        _server.Initialize(options.Value.Port, options.Value.TraceLevel);

        _ = RunAsync(_cts.Token);
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