using Electrify.Dlms.Client.Abstraction;
using Electrify.Models;
using Gurux.DLMS;
using Gurux.DLMS.Objects;
using Gurux.Net;
using Microsoft.Extensions.Logging;

namespace Electrify.Dlms.Client;

public sealed class DlmsClient : IDlmsClient
{
    private readonly Guid _clientId;
    private readonly ILogger<DlmsClient> _logger;
    private readonly GXNet _media;
    private readonly GXDLMSReader _reader;
    private readonly IEnumerable<GXDLMSRegister> _registers;
    private readonly TimeProvider _timeProvider;
    
    public DlmsClient(Guid clientId, ILogger<DlmsClient> logger, GXNet media, GXDLMSReader reader, IEnumerable<GXDLMSRegister> registers, TimeProvider timeProvider)
    {
        _clientId = clientId;
        _logger = logger;
        _media = media;
        _reader = reader;
        _timeProvider = timeProvider;
        _registers = registers;

        _reader.OnNotification += OnNotification;
        
        _media.Open();
        
        _reader.InitializeConnection();
    }

    public IEnumerable<Reading> ReadEnergyProfile(DateTime sinceTime)
    {
        var energyProfile = new GXDLMSProfileGeneric("1.2.3.4.5.6");  // TODO make this configurable not hardcoded logical name
        
        _reader.Read(energyProfile, 3);
        _reader.Read(energyProfile, 2);
        
        var readings = _reader.ReadRowsByRange(energyProfile, sinceTime, _timeProvider.GetLocalNow().DateTime);
        
        foreach (var reading in readings)
        {
            if (reading is object[] r
                && r[0] is GXDateTime gxDateTime
                && r[1] is double energyUsage
                && r[2] is double energyTariff)
            {
                yield return new Reading
                {
                    ClientId = _clientId,
                    DateTime = gxDateTime,
                    EnergyUsage = energyUsage,
                    Tariff = energyTariff
                };
            }
        }
    }

    public void WriteTariff(double tariff)
    {
        var register = _registers.First(r => r.LogicalName == "1.2.3.4.5.7");  // TODO use const or config
        register.Value = tariff;
        _reader.Write(register, 2);
    }

    private void OnNotification(object data)
    {
        _logger.LogInformation("Received DLMS Reader Notification: {Data}", data);
    }
}