using System.Text;
using System.Text.Unicode;
using Electrify.Dlms.Client;
using Electrify.Dlms.Client.Abstraction;
using Electrify.Dlms.Options;
using Electrify.Dlms.Server;
using Electrify.Dlms.Server.Abstraction;
using Gurux.DLMS.Enums;
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
        
        services.AddSingleton<IDlmsClient>( sp =>
        {
            var options = sp.GetRequiredService<DlmsClientOptions>();
            var logger = sp.GetRequiredService<ILogger<DlmsClient>>();
            var media = sp.GetRequiredService<GXNet>();
            var reader = sp.GetRequiredService<GXDLMSReader>();

            var registers = options.LogicalNames.Replace(" ", "").Split(",");
            
            return new DlmsClient(
                logger,
                media,
                reader,
                registers.Select(register => new GXDLMSRegister(register)));
        });

        return services;
    }
    
    public static IServiceCollection AddDlmsServer(
        this IServiceCollection services,
        IConfigurationRoot configuration,
        Action<DlmsServer, IServiceProvider> configure)
    {
        services.Configure<DlmsServerOptions>(configuration.GetSection(nameof(DlmsServerOptions)));

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<DlmsServerOptions>>().Value;
            
            return new GXDLMSAssociationLogicalName
            {
                AuthenticationMechanismName = new GXAuthenticationMechanismName
                {
                    MechanismId = options.Authentication
                },
                Secret = Encoding.UTF8.GetBytes(options.Password),
            };
        });

        services.AddSingleton<GXDLMSClock>();
            
        services.AddSingleton<GXDLMSBase, GXDLMSServerLN_47>();

        services.AddSingleton<IDlmsServer>(sp => new DlmsServer(configure, sp));
        
        return services;
    }
}