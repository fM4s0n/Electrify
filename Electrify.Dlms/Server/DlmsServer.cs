using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Electrify.Dlms.Constants;
using Electrify.Dlms.Models;
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

    public void AddObject(GXDLMSObject dlmsObject, bool writeAccess = false)
    {
        _association.SetAccess3(dlmsObject, 3, AccessMode3.Read);
        _association.SetAccess3(dlmsObject, 2, writeAccess ? AccessMode3.Read | AccessMode3.Write : AccessMode3.Read);
        
        _server.Items.Add(dlmsObject);
    }
    
    public void SetEnergy(double energyValue)
    {
        foreach (GXDLMSObject? dlmsObject in _server.Items)
        {
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

    public IEnumerable<GenericProfileRow> GetReadings()
    {
        var dataFile = GXDLMSBase.GetdataFile();
        using var reader = new StreamReader(dataFile);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null, 
            HeaderValidated = null, 
            Delimiter = ";", 
            Encoding = Encoding.UTF8
        };
        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<GenericProfileRow>().ToList();
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