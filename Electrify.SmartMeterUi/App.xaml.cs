using System.Diagnostics;
using Electrify.Dlms.Options;
using Electrify.Dlms.Server.Abstraction;
using Microsoft.Extensions.Options;
using Serilog;

namespace Electrify.SmartMeterUi;

public partial class App : Application
{
    public App(IDlmsServer dlmsServer, IOptions<DlmsServerOptions> dlmsServerOptions)
    {
        InitializeComponent();

        MainPage = new MainPage();

        Task.Run(() =>
        {
            dlmsServer.Initialise(dlmsServerOptions, dlmsServerOptions.Value.TraceLevel);
        });
    }

    protected override void OnSleep()
    {
        Log.CloseAndFlush();
        base.OnSleep();
    }
}
