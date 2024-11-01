﻿using Microsoft.Extensions.Logging;
using Electrify.AdminUi.Services.Abstractions;
using Serilog;
using Electrify.AdminUi.Services;
using Electrify.Server.Protos;

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
        builder.Services.AddScoped<IAdminService, AdminService>();
        builder.Services.AddGrpcClient<AdminLogin.AdminLoginClient>(options =>
        {
            options.Address = new Uri("http://localhost:5001");
        });

        return builder.Build();
    }
}
