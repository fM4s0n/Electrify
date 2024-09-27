using Electrify.Dlms.Client.Abstraction;
using Gurux.Net;
using Microsoft.Extensions.Logging;

namespace Electrify.Dlms.Client;

public sealed class DlmsClient : IDlmsClient
{
    private readonly ILogger<DlmsClient> _logger;
    private readonly GXNet _media;
    private readonly GXDLMSReader _reader;
    
    public DlmsClient(ILogger<DlmsClient> logger, GXNet media, GXDLMSReader reader)
    {
        _logger = logger;
        _media = media;
        _reader = reader;

        _reader.OnNotification += OnNotification;
        
        _media.Open();
        
        _reader.InitializeConnection();
    }

    private void OnNotification(object data)
    {
        _logger.LogInformation("Received DLMS Reader Notification: {Data}", data);
    }
}