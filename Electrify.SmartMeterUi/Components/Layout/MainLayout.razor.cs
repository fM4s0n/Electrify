using Electrify.Dlms.Options;
using Electrify.Dlms.Server.Abstraction;
using Electrify.Server.ApiClient;
using Electrify.SmartMeterUi.Services.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Extensions.Logging;

namespace Electrify.SmartMeterUi.Components.Layout;

public partial class MainLayout
{
    [Inject] private IHttpClientFactory ClientFactory { get; set; } = default!;
    [Inject] private IOptions<DlmsServerOptions> Options { get; set; } = default!;
    [Inject] private IDlmsServer DlmsServer { get; set; } = default!;
    [Inject] private IErrorMessageService ErrorMessageService { get; set; } = default!;
    
    private ElectrifyApiClient _apiClient = default!;
    
    protected override async Task OnInitializedAsync()
    {
        ILogger<ElectrifyApiClient> logger = new Logger<ElectrifyApiClient>(new SerilogLoggerFactory());
        
        _apiClient = new ElectrifyApiClient(ClientFactory.CreateClient("ElectrifyServer"), logger);
        await Task.WhenAll(InitDlmsServer(), RegisterConnectionWithServer());
    }
    
    private async Task InitDlmsServer()
    {
        Task.Run(() =>
        {
            DlmsServer.Initialise(Options, () => ErrorMessageService.IsConnected = true,
                () => ErrorMessageService.IsConnected =
                    false);
        });
    }

    private async Task RegisterConnectionWithServer()
    {
        await _apiClient.Register(Options.Value.Port, Options.Value.Password, Guid.NewGuid());
    }
}

