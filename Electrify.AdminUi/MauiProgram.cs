﻿using Microsoft.Extensions.Logging;
using Electrify.AdminUi.Services.Abstractions;
using Serilog;
using Electrify.AdminUi.Services;
using Electrify.Server.ApiClient;
using Electrify.Server.ApiClient.Abstraction;

namespace Electrify.AdminUi;

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
            .WriteTo.File(Path.Combine(FileSystem.Current.AppDataDirectory, "electrify-admin-ui.log"))
            .Enrich.FromLogContext().Enrich.WithMachineName().Enrich.WithProperty("ThreadId", Environment.CurrentManagedThreadId);

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
            
        loggerConfiguration.MinimumLevel.Debug();
#endif
        builder.Services.AddSerilog(loggerConfiguration.CreateLogger());
        builder.Services.AddSingleton<IClientService, ClientService>();
        builder.Services.AddSingleton<IAdminService, AdminService>();
        builder.Services.AddSingleton<IBillService, BillService>();
        builder.Services.AddSingleton<IGreetingService, GreetingService>();
        builder.Services.AddSingleton<IConnectedClientsService, ConnectedClientsService>();
        builder.Services.AddSingleton(TimeProvider.System);

        builder.Services.AddSingleton<IElectrifyApiClient>(sp => new ElectrifyApiClient(
            new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8888")
            },
            sp.GetRequiredService<ILogger<ElectrifyApiClient>>()
        ));
        
        return builder.Build();
    }
}
