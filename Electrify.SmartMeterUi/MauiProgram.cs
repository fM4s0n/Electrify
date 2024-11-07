using Electrify.SmartMeterUi.Services;
using Electrify.SmartMeterUi.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Electrify.SmartMeterUi;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

		LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Debug()
            .WriteTo.File(Path.Combine(FileSystem.Current.AppDataDirectory, "electrify-smartMeter-ui.log"))
            .Enrich.FromLogContext().Enrich.WithMachineName().Enrich.WithProperty("ThreadId", Environment.CurrentManagedThreadId);
		
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();

		loggerConfiguration.MinimumLevel.Debug();
#endif
		builder.Services.AddSerilog(loggerConfiguration.CreateLogger());
        builder.Services.AddSingleton<IUsageService, UsageService>();

        return builder.Build();
	}
}
