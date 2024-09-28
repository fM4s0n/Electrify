using Electrify.Dlms.Client.Abstraction;
using Gurux.DLMS.Objects;
using Gurux.Net;
using Microsoft.Extensions.Logging;

namespace Electrify.Dlms.Client;

public sealed class DlmsClient : IDlmsClient
{
    private readonly ILogger<DlmsClient> _logger;
    private readonly GXNet _media;
    private readonly GXDLMSReader _reader;

    private List<GXDLMSRegister> _registers = [];
    
    public DlmsClient(ILogger<DlmsClient> logger, GXNet media, GXDLMSReader reader, List<GXDLMSRegister> registers)
    {
        _logger = logger;
        _media = media;
        _reader = reader;

        _reader.OnNotification += OnNotification;
        
        _media.Open();
        
        _reader.InitializeConnection();
        
        _registers = registers;
    }

    public void ReadEnergy()
    {
        _reader.Read(_registers.Single(register => register.LogicalName == "1.1.1.8.0.255"), 2);
    }

    public void SetEnergy(int energyValue)
    {
        _registers.Single(register => register.LogicalName == "1.1.1.8.0.255").Value = energyValue;
        _reader.Write(_registers.Single(register => register.LogicalName == "1.1.1.8.0.255"), 2);
    }

    private void OnNotification(object data)
    {
        _logger.LogInformation("Received DLMS Reader Notification: {Data}", data);
    }
}