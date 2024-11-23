using Electrify.Dlms.Options;
using Electrify.Dlms.Server.Abstraction;
using Electrify.Server.ApiClient;
using Microsoft.Extensions.Options;
using Serilog;
using Electrify.SmartMeterUi.Services.Abstractions;

namespace Electrify.SmartMeterUi;

public partial class App : Application
{
    public App(IDlmsServer dlmsServer, IOptions<DlmsServerOptions> dlmsServerOptions, IErrorMessageService errorMessageService, IHttpClientFactory httpClientFactory)
    {
        InitializeComponent();
        MainPage = new MainPage();
    }

    protected override void OnSleep()
    {
        Log.CloseAndFlush();
        base.OnSleep();
    }
}
