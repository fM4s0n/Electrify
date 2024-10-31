using Electrify.Dlms.Options;
using Electrify.Dlms.Server.Abstraction;
using Microsoft.Extensions.Options;
using Serilog;
using System.Diagnostics;

namespace Electrify.SmartMeterUi;

public partial class App : Application
{
    public App(IDlmsServer dlmsServer, IOptions<DlmsServerOptions> options, TraceLevel traceLevel)
    {
        InitializeComponent();
        option
        MainPage = new MainPage(dlmsServer, options, traceLevel);
    }

    protected override void OnSleep()
    {
        Log.CloseAndFlush();
        base.OnSleep();
    }
}
