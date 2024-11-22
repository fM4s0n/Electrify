using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Electrify.Dlms.Client;
using Electrify.Dlms.Constants;
using Electrify.Dlms.Options;
using Electrify.Dlms.Server;
using FluentAssertions;
using Gurux.DLMS;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Secure;
using Gurux.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Electrify.E2ETests;

public class EndToEndTests : IDisposable
{
    private readonly DlmsServer _dlmsServer;
    private readonly DlmsClient _dlmsClient;
    private Timer? _usageTimer;
    private readonly Random _random;
    
    public EndToEndTests()
    {
        _random = new Random();
        
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        
        var serverServiceProvider = ConfigureServer(port).BuildServiceProvider();
        var clientServiceProvider = ConfigureClient(port).BuildServiceProvider();

        _dlmsServer = CreateAndInitialiseServer(serverServiceProvider);
        
        Thread.Sleep(100);  // make sure server finished initialisation before trying to connect to it
        
        _dlmsClient = CreateAndInitialiseClient(clientServiceProvider);
    }

    private static ServiceCollection ConfigureServer(int port)
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
            Password = "SuperSecretTestPassword",
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

    private static ServiceCollection ConfigureClient(int port)
    {
        var services = new ServiceCollection();

        services.AddSingleton(TimeProvider.System);
        
        services.AddLogging(configure =>
        {
            configure.AddConsole();
            configure.SetMinimumLevel(LogLevel.None);
        });
        
        services.AddSingleton(Options.Create(new DlmsClientOptions
        {
            UseLogicalNameReferencing = true,
            ClientAddress = 16,
            ServerAddress = 1,
            Authentication = Authentication.HighSHA256,
            Password = "SuperSecretTestPassword",
            InterfaceType = InterfaceType.WRAPPER,
            Protocol = NetworkType.Tcp,
            ServerHostname = "127.0.0.1",
            ServerPort = port,
            InvocationCounter = "0.0.43.1.8.255",
            LogicalNames = [RegisterNames.EnergyUsage, RegisterNames.EnergyTariff],
        }));
        
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<DlmsClientOptions>>().Value;
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
            var options = sp.GetRequiredService<IOptions<DlmsClientOptions>>().Value;
            return new GXNet(options.Protocol, options.ServerHostname, options.ServerPort);
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<DlmsClientOptions>>().Value;
            var client = sp.GetRequiredService<GXDLMSSecureClient>();
            var media = sp.GetRequiredService<GXNet>();
            
            return new GXDLMSReader(client, media, TraceLevel.Off, options.InvocationCounter);
        });

        return services;
    }

    private DlmsServer CreateAndInitialiseServer(IServiceProvider serviceProvider)
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

            server.AddObject(energyRegister);
            server.AddObject(tariffRegister, true);
            server.AddObject(energyProfile);

            _usageTimer = new Timer(_ =>
            {
                if (energyRegister.Value is not double oldValue)
                {
                    throw new Exception("Energy Register should store a double");
                }

                var randomDouble = _random.NextDouble();
                if (randomDouble == 0)
                {
                    randomDouble = 0.001;
                }
                
                energyRegister.Value = oldValue + randomDouble;
            }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(300));
        }

        var server = new DlmsServer(Configure, serviceProvider);

        Task.Run(() =>
        {
            server.Initialise(
                serviceProvider.GetRequiredService<IOptions<DlmsServerOptions>>(),
                () => {},
                () => {});
        });

        return server;
    }

    private static DlmsClient CreateAndInitialiseClient(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<IOptions<DlmsClientOptions>>().Value;
        var logger = serviceProvider.GetRequiredService<ILogger<DlmsClient>>();
        var media = serviceProvider.GetRequiredService<GXNet>();
        var reader = serviceProvider.GetRequiredService<GXDLMSReader>();
        var timeProvider = serviceProvider.GetRequiredService<TimeProvider>();

        var registers = options.LogicalNames.Select(register => new GXDLMSRegister(register));
            
        return new DlmsClient(
            Guid.NewGuid(),
            logger,
            media,
            reader,
            registers,
            timeProvider);
    }
    
    [Fact]
    public async Task EnergyUsage_In_Readings_Should_Increase_Over_Time()
    {
        var localStartTime = DateTime.Now;
        
        await Task.Delay(4_000);
        
        var readings = _dlmsClient.ReadEnergyProfile(localStartTime).ToList();
        var localEndTime = DateTime.Now;

        readings.Should().NotBeEmpty();
        
        for (var i = 0; i < readings.Count; i++)
        {
            var reading = readings[i];
            reading.DateTime.Should().BeWithin(localEndTime - localStartTime);

            if (i == 0)
            {
                continue;
            }

            var lastReading = readings[i-1];
            reading.EnergyUsage.Should().BeGreaterThan(lastReading.EnergyUsage);
        }
    }
    
    [Fact]
    public async Task Tariff_In_Readings_Should_Change_When_Written_To()
    {
        var localStartTime = DateTime.Now;
        
        await Task.Delay(1_000);
        
        _dlmsClient.WriteTariff(_random.Next(1, 10) + _random.NextDouble());

        await Task.Delay(1_000);
        
        var readings = _dlmsClient.ReadEnergyProfile(localStartTime).ToList();
        var localEndTime = DateTime.Now;

        readings.Should().NotBeEmpty();
        
        foreach (var reading in readings)
        {
            reading.DateTime.Should().BeWithin(localEndTime - localStartTime);
        }

        readings.GroupBy(r => r.Tariff).Count().Should().BeGreaterThan(1);
    }
    
    public void Dispose()
    {
        _dlmsServer.Dispose();
        _usageTimer?.Dispose();
        GC.SuppressFinalize(this);
    }
}