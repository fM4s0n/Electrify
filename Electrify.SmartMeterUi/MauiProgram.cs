using System.Diagnostics;
using Blazored.Toast;
using Electrify.Dlms.Constants;
using Electrify.Dlms.Extensions;
using Electrify.Dlms.Options;
using Electrify.SmartMeterUi.Services;
using Electrify.SmartMeterUi.Services.Abstractions;
using Gurux.DLMS;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

		builder.Services.AddSingleton(Options.Create(new DlmsServerOptions
		{
			Port = 4059,
			Password = "YourSuperSecureSecretKey1234567890",
			Authentication = Authentication.HighSHA256,
			TraceLevel = TraceLevel.Verbose,
		}));
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();

		loggerConfiguration.MinimumLevel.Debug();
#endif
		builder.Services.AddSerilog(loggerConfiguration.CreateLogger());
        builder.Services.AddSingleton<IUsageService, UsageService>();
		builder.Services.AddBlazoredToast();

        builder.Services.AddDlmsServer(builder.Configuration, (server, sp) =>
		{
			var clock = sp.GetRequiredService<GXDLMSClock>();
			
			var energyRegister = new GXDLMSRegister(RegisterNames.EnergyUsage)
			{
				Scaler = 1.0,
				Unit = Unit.ActiveEnergy,
				Value = 123.45,
			};

			var tariffRegister = new GXDLMSRegister(RegisterNames.EnergyTariff)
			{
				Scaler = 1.0,
				Unit = Unit.LocalCurrency,
				Value = 24.50,
			};

			var energyProfile = new GXDLMSProfileGeneric(RegisterNames.EnergyProfile)
			{
				CapturePeriod = 1,  // TODO this every second use config instead
				SortObject = clock,
				CaptureObjects =
				[
					new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(clock, new GXDLMSCaptureObject(2, 0)),
					new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(energyRegister, new GXDLMSCaptureObject(2, 0)),
					new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(tariffRegister, new GXDLMSCaptureObject(2, 0)),
				]
			};
			
			server.AddObject(energyRegister);
			server.AddObject(tariffRegister, AccessMode3.Write);
			server.AddObject(energyProfile);
		});
		
        return builder.Build();
	}
}
