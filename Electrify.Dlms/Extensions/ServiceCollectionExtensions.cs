using System.Text;
using Electrify.Dlms.Options;
using Electrify.Dlms.Server;
using Electrify.Dlms.Server.Abstraction;
using Gurux.DLMS.Objects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Electrify.Dlms.Extensions;

public static class ServiceCollectionExtensions
{
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