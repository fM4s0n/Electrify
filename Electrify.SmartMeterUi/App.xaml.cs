using Electrify.Dlms.Options;
using Electrify.Dlms.Server.Abstraction;
using Microsoft.Extensions.Options;
using Serilog;
using Electrify.SmartMeterUi.Services.Abstractions;
namespace Electrify.SmartMeterUi;

public partial class App : Application
{
    public App(IDlmsServer dlmsServer, IOptions<DlmsServerOptions> dlmsServerOptions, IErrorMessageService errorMessageService)
    {
        InitializeComponent();

        MainPage = new MainPage();

        Task.Run(() =>
        {
            dlmsServer.Initialise(
                dlmsServerOptions,
                () => errorMessageService.IsConnected = true,
                () => errorMessageService.IsConnected = false);
        });
    }

    protected override void OnSleep()
    {
        Log.CloseAndFlush();
        base.OnSleep();
    }
}
