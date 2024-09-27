using Electrify.Dlms.Client;
using Electrify.Dlms.Client.Abstraction;
using Electrify.Dlms.Options;
using Electrify.Dlms.Server;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Secure;
using Gurux.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Electrify.Dlms.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDlmsClient(
        this IServiceCollection services,
        IConfigurationRoot configuration,
        LogLevel logLevel)
    {
        services.Configure<DlmsClientOptions>(configuration.GetSection(nameof(DlmsClientOptions)));
        
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<DlmsClientOptions>();
            return new GXDLMSSecureClient(
                options.UseLogicalNameReferencing,
                options.ClientAddress,
                options.ServerAddress,
                options.Authentication,
                options.Password,
                options.InterfaceType);
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<DlmsClientOptions>();
            return new GXNet(options.Protocol, options.ServerHostname, options.ServerPort);
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<DlmsClientOptions>();
            var client = sp.GetRequiredService<GXDLMSSecureClient>();
            var media = sp.GetRequiredService<GXNet>();
            
            return new GXDLMSReader(client, media, logLevel.ToTraceLevel(), options.InvocationCounter);
        });
        
        services.AddSingleton<IDlmsClient, DlmsClient>();

        return services;
    }
    
    public static IServiceCollection AddDlmsServer(
        this IServiceCollection services,
        IConfigurationRoot configuration,
        Action<DlmsServer> configure,
        LogLevel logLevel)
    {
        services.Configure<DlmsServerOptions>(configuration.GetSection(nameof(DlmsServerOptions)));

        services.AddSingleton(new GXDLMSAssociationLogicalName());
            
        services.AddSingleton<GXDLMSBase, GXDLMSServerLN_47>();

        services.AddSingleton(sp =>
        {
            var association = sp.GetRequiredService<GXDLMSAssociationLogicalName>();
            var serverBase = sp.GetRequiredService<GXDLMSBase>();
            var options = sp.GetRequiredService<IOptions<DlmsServerOptions>>();
            var logger = sp.GetRequiredService<ILogger<DlmsServer>>();

            return new DlmsServer(association, serverBase, configure, options, logLevel.ToTraceLevel(), logger);
        });
        
        return services;
    }
}