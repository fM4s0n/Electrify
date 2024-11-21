using Electrify.Dlms.Extensions;
using Electrify.Dlms.Options;
using Electrify.Dlms.Server;
using Electrify.Dlms.Server.Abstraction;
using FluentAssertions;
using FluentAssertions.Execution;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Electrify.Dlms.UnitTests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
        public void AddDlmsServer_Should_Register_All_Services_Correctly()
        {
            // Arrange
            var services = new ServiceCollection();

            services.AddLogging(options =>
            {
                options.AddConsole();
                options.SetMinimumLevel(LogLevel.None);
            });
            
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "DlmsServerOptions:Authentication", "HighSHA256" },
                    { "DlmsServerOptions:Password", "TestPassword" },
                }!)
                .Build();

            var configure = Substitute.For<Action<DlmsServer, IServiceProvider>>();

            // Act
            services.AddDlmsServer(configuration, configure);

            var serviceProvider = services.BuildServiceProvider();
            
            var options = serviceProvider.GetRequiredService<IOptions<DlmsServerOptions>>().Value;
            var associationLogicalName = serviceProvider.GetRequiredService<GXDLMSAssociationLogicalName>();
            var clock = serviceProvider.GetRequiredService<GXDLMSClock>();
            var dlmsBase = serviceProvider.GetRequiredService<GXDLMSBase>();
            var dlmsServer = serviceProvider.GetRequiredService<IDlmsServer>();

            // Assert
            options.Should().NotBeNull();
            using (new AssertionScope())
            {
                options.Authentication.Should().Be(Authentication.HighSHA256);
                options.Password.Should().Be("TestPassword");
            }

            associationLogicalName.Should().NotBeNull();
            associationLogicalName.AuthenticationMechanismName.Should().NotBeNull();

            using (new AssertionScope())
            {
                associationLogicalName.AuthenticationMechanismName.MechanismId.Should().Be(Authentication.HighSHA256);
                associationLogicalName.Secret.Should().BeEquivalentTo("TestPassword"u8.ToArray());
            }
            
            clock.Should().NotBeNull();

            dlmsBase.Should().NotBeNull();
            dlmsBase.GetType().Should().Be<GXDLMSServerLN_47>();

            dlmsServer.Should().NotBeNull();
            dlmsServer.GetType().Should().Be<DlmsServer>();
            
            configure.Received(1).Invoke(Arg.Any<DlmsServer>(), Arg.Any<IServiceProvider>());
        }
}