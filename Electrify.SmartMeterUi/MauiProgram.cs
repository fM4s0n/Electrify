using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Electrify.Dlms.Constants;
using Electrify.Dlms.Extensions;
using Electrify.Dlms.Options;
using Electrify.Server.ApiClient;
using Electrify.Server.ApiClient.Abstraction;
using Electrify.SmartMeterUi.Services;
using Electrify.SmartMeterUi.Services.Abstraction;
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
		
		var listener = new TcpListener(IPAddress.Loopback, 0);
		listener.Start();
		var port = ((IPEndPoint)listener.LocalEndpoint).Port;
		listener.Stop();
		
		builder.Services.AddSingleton(Options.Create(new DlmsServerOptions
		{
			Port = port,
			Password = "YourSuperSecureSecretKey1234567890",
			Authentication = Authentication.HighSHA256,
			TraceLevel = TraceLevel.Verbose,
			GenericProfileCapturePeriod = 5,
		}));
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();

		loggerConfiguration.MinimumLevel.Debug();
#endif
		builder.Services.AddSingleton<IElectrifyApiClient>(sp => new ElectrifyApiClient(
			new HttpClient
			{
				BaseAddress = new Uri("http://localhost:8888")
			},
			sp.GetRequiredService<ILogger<ElectrifyApiClient>>()
		));
		
		builder.Services.AddSerilog(loggerConfiguration.CreateLogger());
        builder.Services.AddSingleton<IUsageService, UsageService>();

        builder.Services.AddSingleton<ICommandLineArgsProvider, CommandLineArgsProvider>();
		builder.Services.AddSingleton<IErrorMessageService, ErrorMessageService>();
		builder.Services.AddScoped<IConnectionService, ConnectionService>();
		
        builder.Services.AddDlmsServer(builder.Configuration, (server, sp) =>
		{
			var clock = sp.GetRequiredService<GXDLMSClock>();
			var options = sp.GetRequiredService<IOptions<DlmsServerOptions>>().Value;
			
			var energyRegister = new GXDLMSRegister(RegisterNames.EnergyUsage)
			{
				Scaler = 1.0,
				Unit = Unit.ActiveEnergy,
				Value = 0,
			};

			var tariffRegister = new GXDLMSRegister(RegisterNames.EnergyTariff)
			{
				Scaler = 1.0,
				Unit = Unit.LocalCurrency,
				Value = 0.1,
			};
			
			var errorMessageRegister = new GXDLMSRegister(RegisterNames.ErrorMessage)
			{
				Scaler = 1.0,
				Unit = Unit.NoUnit,
				Value = string.Empty,
			};

			var energyProfile = new GXDLMSProfileGeneric(RegisterNames.EnergyProfile)
			{
				CapturePeriod = options.GenericProfileCapturePeriod,
				SortObject = clock,
				CaptureObjects =
				[
					new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(clock, new GXDLMSCaptureObject(2, 0)),
					new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(energyRegister, new GXDLMSCaptureObject(2, 0)),
					new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(tariffRegister, new GXDLMSCaptureObject(2, 0)),
				]
			};
			
			server.AddObject(energyRegister);
			server.AddObject(tariffRegister, true);
			server.AddObject(energyProfile);
			server.AddObject(errorMessageRegister, true);
		});
		
        return builder.Build();
	}
}
