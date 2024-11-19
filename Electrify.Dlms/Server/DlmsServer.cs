using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using Electrify.Dlms.Constants;
using Electrify.Dlms.Models;
using Electrify.Dlms.Options;
using Electrify.Dlms.Server.Abstraction;
using Electrify.SmartMeterUi.Services.Abstractions;
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
    private readonly IErrorMessageService _errorMessageService;

    public DlmsServer(Action<DlmsServer, IServiceProvider> configure, IServiceProvider serviceProvider)
    {
        _association = serviceProvider.GetRequiredService<GXDLMSAssociationLogicalName>();
        _server = serviceProvider.GetRequiredService<GXDLMSBase>();
        _logger = serviceProvider.GetRequiredService<ILogger<DlmsServer>>();
        _errorMessageService = serviceProvider.GetRequiredService<IErrorMessageService>();

        configure.Invoke(this, serviceProvider);
    }

    public void AddObject(GXDLMSObject dlmsObject, bool writeAccess = false)
    {
        _association.SetAccess3(dlmsObject, 3, AccessMode3.Read);
        _association.SetAccess3(dlmsObject, 2, writeAccess ? AccessMode3.Read | AccessMode3.Write : AccessMode3.Read);
        
        _server.Items.Add(dlmsObject);
    }
    
    public void SetEnergy(int energyValue)
    {
        foreach (GXDLMSObject? dlmsObject in _server.Items)
        {
            if (dlmsObject is GXDLMSRegister { LogicalName: RegisterNames.EnergyUsage } register)
            {
                register.Value = energyValue;
            }
        }
    }

    public void Initialise(IOptions<DlmsServerOptions> options, TraceLevel traceLevel)
    {
        // set up callabcks for connection status
        _server.OnConnectedCallback = () => _errorMessageService.IsConnected = true;
        _server.OnDisconnectedCallback = () => _errorMessageService.IsConnected = false;

        _server.Initialize(options.Value.Port, traceLevel);

        _ = RunAsync(_cts.Token);
    }

    public IEnumerable<GenericProfileRow> GetReadings()
    {
        var dataFile = GXDLMSBase.GetdataFile();
        
        using var reader = new StreamReader(dataFile);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        
        return csv.GetRecords<GenericProfileRow>();
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