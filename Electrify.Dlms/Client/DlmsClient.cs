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

    public double ReadEnergy()
    {
        GXDLMSRegister register = _registers.Single(register => register.LogicalName == "1.1.1.8.0.255");

        if (register.Unit == default)
        {
            _reader.Read(register, 3);
        }
        
        _reader.Read(register, 2);
        
        return (double)register.Value;
    }

    private void OnNotification(object data)
    {
        _logger.LogInformation("Received DLMS Reader Notification: {Data}", data);
    }
}