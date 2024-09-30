using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;  // For PlatformView

namespace Electrify.SmartMeterUi;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        SetMainWindowSize();
    }

    /// <summary>
    /// Sets the default fixed screen size for the application
    /// </summary>
    private void SetMainWindowSize()
    {
        const float width = 1024f;
        const float height = 600f;

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
#elif WINDOWS
                var nativeWindow = handler.PlatformView;
                if (nativeWindow != null)
                {
                    Task.Run(() =>
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            var windowHandle = nativeWindow.GetWindowHandle();
                            if (windowHandle != IntPtr.Zero)
                            {
                                // Use Windows API to set the size
                                var mauiWindow = handler.VirtualView;
                                mauiWindow.Width = width;
                                mauiWindow.Height = height;
                            }
                        });
                    });
                }
#endif
            });
    }
}
