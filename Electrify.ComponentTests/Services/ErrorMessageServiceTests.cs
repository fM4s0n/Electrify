using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Electrify.Dlms.Constants;
using Electrify.Dlms.Options;
using Electrify.Dlms.Server;
using Electrify.Models;
using FluentAssertions;
using Gurux.DLMS;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Task = System.Threading.Tasks.Task;

namespace Electrify.ComponentTests.Services;

public class ErrorMessageServiceTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    [Fact]
    public async Task ErrorMessage_Should_Trigger_Error_Message_Update_Invocation()
    {
        // Arrange
        var secret = Guid.NewGuid().ToString();

        Client client = fixture.CreateEClient();

        await fixture.Database.SaveChangesAsync();
        
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        
        var serverServiceProvider = ConfigureServer(port, secret).BuildServiceProvider();

        var errorCallback = Substitute.For<Action>();
        
        CreateAndInitialiseServer(serverServiceProvider, errorCallback);

        await Task.Delay(100);  // make sure server finished initialisation before trying to connect to it
        
        var response = await fixture.ApiClient.Register(port, secret, client.Id);
        response.Success.Should().BeTrue();

        await Task.Delay(50);
        
        // Act
        await fixture.ApiClient.ErrorMessage();

        await Task.Delay(50);
        
        // Assert
        errorCallback.Received(1).Invoke();
    }
    
    [Fact]
    public async Task ErrorMessage_Should_NotTrigger_Error_Message_Update_Invocation_When_Not_Called()
    {
        // Arrange
        var secret = Guid.NewGuid().ToString();
        
        Client client = fixture.CreateEClient();
        
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        
        var serverServiceProvider = ConfigureServer(port, secret).BuildServiceProvider();

        var errorCallback = Substitute.For<Action>();
        
        CreateAndInitialiseServer(serverServiceProvider, errorCallback);

        await Task.Delay(100);  // make sure server finished initialisation before trying to connect to it
        
        var response = await fixture.ApiClient.Register(port, secret, client.Id);
        response.Success.Should().BeTrue();

        await Task.Delay(50);
        
        // Assert
        errorCallback.Received(0).Invoke();
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
    
    private DlmsServer CreateAndInitialiseServer(IServiceProvider serviceProvider,  Action onErrorMessageUpdateCallback)
    {
        void Configure(DlmsServer server, IServiceProvider sp)
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
            
            var errorMessageRegister = new GXDLMSRegister(RegisterNames.ErrorMessage)
            {
                Scaler = 1.0,
                Unit = Unit.NoUnit,
                Value = string.Empty,
            };

            server.AddObject(energyRegister);
            server.AddObject(tariffRegister, true);
            server.AddObject(energyProfile);
            server.AddObject(errorMessageRegister, true);
        }

        var server = new DlmsServer(Configure, serviceProvider);

        Task.Run(() =>
        {
            server.Initialise(
                serviceProvider.GetRequiredService<IOptions<DlmsServerOptions>>(),
                () => {},
                () => {},
                onErrorMessageUpdateCallback);
        });

        return server;
    }
}