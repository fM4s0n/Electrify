using Electrify.Dlms.Options;
using Electrify.Dlms.Server.Abstraction;
using Microsoft.Extensions.Options;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using System.Diagnostics;  // For PlatformView

namespace Electrify.SmartMeterUi;

public partial class MainPage : ContentPage
{
    public MainPage(IDlmsServer dlmsServer, IOptions<DlmsServerOptions> options, TraceLevel traceLevel)
    {
        InitializeComponent();

        dlmsServer.Initialise(options, traceLevel);

        SetMainWindowSize();
    }

    /// <summary>
    /// Sets the default fixed screen size for the application
    /// </summary>
    private void SetMainWindowSize()
    {
    #if IOS || MACCATALYST
        const float width = 1024f;
        const float height = 600f;
        SetMacWindowSize(width, height);
    #endif
    }

    private void SetMacWindowSize(float width, float height)
    {
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(
            nameof(IWindow), (handler, _) =>
            {
            #if IOS || MACCATALYST
                if (handler.PlatformView.WindowScene is { SizeRestrictions: not null })
                {
                    Task.Run(() =>
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            handler.PlatformView.WindowScene.SizeRestrictions.MinimumSize =
                                new CoreGraphics.CGSize(width, height);
                            handler.PlatformView.WindowScene.SizeRestrictions.MaximumSize =
                                new CoreGraphics.CGSize(width, height);
                        });
                    });
                }
            #endif
            });
    }
}
