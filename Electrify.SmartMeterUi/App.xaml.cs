using Electrify.Dlms.Options;
using Electrify.Dlms.Server.Abstraction;
using Electrify.Server.ApiClient.Abstraction;
using Electrify.SmartMeterUi.Services.Abstraction;
using Microsoft.Extensions.Options;
using Serilog;

namespace Electrify.SmartMeterUi;

public partial class App : Application
{
    public App(IDlmsServer dlmsServer, IOptions<DlmsServerOptions> dlmsServerOptions, IErrorMessageService errorMessageService, IElectrifyApiClient electrifyApiClient)
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
