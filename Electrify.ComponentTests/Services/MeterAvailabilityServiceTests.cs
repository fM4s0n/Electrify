using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Electrify.Dlms.Constants;
using Electrify.Dlms.Options;
using Electrify.Dlms.Server;
using Electrify.Protos;
using FluentAssertions;
using Grpc.Core;
using Gurux.DLMS;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Authentication = Gurux.DLMS.Enums.Authentication;
using Task = System.Threading.Tasks.Task;

namespace Electrify.ComponentTests.Services;

public class MeterAvailabilityServiceTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    [Fact]
    public async Task Register_Should_Return_Success_If_Port_Open()
    {
        // Arrange
        var secret = Guid.NewGuid().ToString();
        var clientId = Guid.NewGuid();
        
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        
        var serverServiceProvider = ConfigureServer(port, secret).BuildServiceProvider();

        CreateAndInitialiseServer(serverServiceProvider);
        
        Thread.Sleep(100);  // make sure server finished initialisation before trying to connect to it
        
        // Act
        var response = await fixture.ApiClient.Register(port, secret, clientId);
        
        // Assert
        response.Success.Should().BeTrue();
    }
    
    [Fact]
    public async Task Register_Should_Be_Unsuccessful_If_Cannot_Connect_To_Port()
    {
        // Arrange
        const int port = 999999;
        var secret = Guid.NewGuid().ToString();
        var clientId = Guid.NewGuid();
        
        // Act
        var response = await fixture.ApiClient.Register(port, secret, clientId);
        
        // Assert
        response.Success.Should().BeFalse();
    }
    
    // TODO not SOLID this is copy pasted from E2E tests, make sure password matches if combining though
    private IServiceCollection ConfigureServer(int port, string password)
    {
        var services = new ServiceCollection();

        services.AddLogging(configure =>
        {
            configure.AddConsole();
            configure.SetMinimumLevel(LogLevel.None);
        });
        
        services.AddSingleton(Options.Create(new DlmsServerOptions
        {
            Port = port,
            Password = password,
            Authentication = Authentication.HighSHA256,
            TraceLevel = TraceLevel.Off,
        }));
        
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

        return services;
    }
    
    private Dlms.Server.DlmsServer CreateAndInitialiseServer(IServiceProvider serviceProvider)
    {
        void Configure(Dlms.Server.DlmsServer server, IServiceProvider sp)
        {
            var clock = sp.GetRequiredService<GXDLMSClock>();

            var energyRegister = new GXDLMSRegister(RegisterNames.EnergyUsage)
            {
                Scaler = 1.0,
                Unit = Unit.ActiveEnergy,
                Value = 0.0,
            };

            var tariffRegister = new GXDLMSRegister(RegisterNames.EnergyTariff)
            {
                Scaler = 1.0,
                Unit = Unit.LocalCurrency,
                Value = 0.0,
            };

            var energyProfile = new GXDLMSProfileGeneric(RegisterNames.EnergyProfile)
            {
                CapturePeriod = 1, // TODO this every second use config instead
                SortObject = clock,
                CaptureObjects = [new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(clock, new GXDLMSCaptureObject(2, 0)), new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(energyRegister, new GXDLMSCaptureObject(2, 0)), new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(tariffRegister, new GXDLMSCaptureObject(2, 0)),]
            };

            server.AddObject(energyRegister);
            server.AddObject(tariffRegister, true);
            server.AddObject(energyProfile);
        }

        var server = new Dlms.Server.DlmsServer(Configure, serviceProvider);

        Task.Run(() =>
        {
            server.Initialise(
                serviceProvider.GetRequiredService<IOptions<DlmsServerOptions>>(),
                () => {},
                () => {});
        });

        return server;
    }
}