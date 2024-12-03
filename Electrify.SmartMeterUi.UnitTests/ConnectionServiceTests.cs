using Electrify.Dlms.Options;
using Electrify.Dlms.Server.Abstraction;
using Electrify.Protos;
using Electrify.Server.ApiClient.Abstraction;
using Electrify.Server.ApiClient.Contracts;
using Electrify.SmartMeterUi.Services;
using Electrify.SmartMeterUi.Services.Abstraction;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Electrify.SmartMeterUi.UnitTests;

public class ConnectionServiceTests
{
    private readonly IElectrifyApiClient _apiClientMock;
    private readonly IOptions<DlmsServerOptions> _options;
    private readonly IDlmsServer _dlmsServerMock;
    private readonly ICommandLineArgsProvider _argsProviderMock;
    private readonly ConnectionService _connectionService;

    public ConnectionServiceTests()
    {
        _apiClientMock = Substitute.For<IElectrifyApiClient>();

        _apiClientMock.Register(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Guid>())
            .Returns(new HttpAvailabilityResponse
            {
                Success = true,
                HistoricReadings = [],
            });

        _options = Options.Create(new DlmsServerOptions
        {
            Port = 1234,
            Password = "securepassword"
        });

        _dlmsServerMock = Substitute.For<IDlmsServer>();
        _argsProviderMock = Substitute.For<ICommandLineArgsProvider>();

        var errorMessageServiceMock = Substitute.For<IErrorMessageService>();
        
        _connectionService = new ConnectionService(
            _apiClientMock,
            _options,
            _dlmsServerMock,
            errorMessageServiceMock,
            _argsProviderMock,
            Substitute.For<IUsageService>()
        );
    }

    [Fact]
    public async Task InitializeConnectionAsync_ShouldOnlyRegister_WhenReconnect()
    {
        // Arrange
        var validGuid = Guid.NewGuid();
        _argsProviderMock.GetArgAtIndex(1).Returns(validGuid.ToString());

        // Act
        await _connectionService.InitializeConnectionAsync(isReconnect: true);

        // Assert
        _dlmsServerMock.DidNotReceive().Initialise(
            Arg.Any<IOptions<DlmsServerOptions>>(),
            Arg.Any<Action>(),
            Arg.Any<Action>(),
            Arg.Any<Action>()
        );

        await _apiClientMock.Received(1).Register(_options.Value.Port, _options.Value.Password, validGuid);

        _connectionService.InitialConnectionMade.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterConnectionWithServer_ShouldRegisterSuccessfully_WithValidClientId()
    {
        // Arrange
        var validGuid = Guid.NewGuid();
        _argsProviderMock.GetArgAtIndex(1).Returns(validGuid.ToString());

        // Act
        await _connectionService.InitializeConnectionAsync(isReconnect: false);

        // Assert
        await _apiClientMock.Received(1).Register(_options.Value.Port, _options.Value.Password, validGuid);
        _connectionService.InitialConnectionMade.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterConnectionWithServer_ShouldThrowException_WithInvalidClientId()
    {
        // Arrange
        _argsProviderMock.GetArgAtIndex(1).Returns("invalid-guid");

        // Act
        var act = async () => await _connectionService.InitializeConnectionAsync(isReconnect: false);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Invalid ClientId specified in command line arguments");
    }
}