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
		
		builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
		
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();

		loggerConfiguration.MinimumLevel.Debug();
#endif
		builder.Services.AddSerilog(loggerConfiguration.CreateLogger());
        builder.Services.AddSingleton<IUsageService, UsageService>();


		builder.Services.AddDlmsServer(builder.Configuration, (server, sp) =>
		{
			var clock = sp.GetRequiredService<GXDLMSClock>();
			
			var energyRegister = new GXDLMSRegister(RegisterNames.EnergyUsage)
			{
				Scaler = 1.0,
				Unit = Unit.ActiveEnergy,
				Value = 123.45,
			};

			var energyProfile = new GXDLMSProfileGeneric(RegisterNames.EnergyProfile)
			{
				CapturePeriod = 1,  // TODO this every second use config instead
				SortObject = clock,
				CaptureObjects =
				[
					new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(clock, new GXDLMSCaptureObject(2, 0)),
					new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(energyRegister, new GXDLMSCaptureObject(2, 0))
				]
			};
			
			server.AddObject(energyRegister);
			server.AddObject(energyProfile);
		});
		
        return builder.Build();
	}
}
