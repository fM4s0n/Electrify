using Electrify.Dlms.Client;
using Electrify.Dlms.Options;
using Electrify.Protos;
using Electrify.Server.Options;
using Electrify.Server.Services;
using Electrify.Server.Services.Abstraction;
using FluentAssertions;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Electrify.Server.UnitTests.Services;

public class MeterAvailabilityServiceTests
{
    private readonly IOptions<DlmsClientOptions> _dlmsClientOptions = Substitute.For<IOptions<DlmsClientOptions>>();
    private readonly IOptions<ObservabilityOptions> _observabilityOptions = Substitute.For<IOptions<ObservabilityOptions>>();
    private readonly ILogger<DlmsClient> _dlmsClientLogger = Substitute.For<ILogger<DlmsClient>>();
    private readonly ILogger<MeterAvailabilityService> _logger = Substitute.For<ILogger<MeterAvailabilityService>>();
    private readonly TimeProvider _timeProvider = Substitute.For<TimeProvider>();
    private readonly IDlmsClientService _dlmsClientService = Substitute.For<IDlmsClientService>();

    private MeterAvailabilityService _meterAvailabilityService;
    
    public MeterAvailabilityServiceTests()
    {
        _meterAvailabilityService = new MeterAvailabilityService(
            _dlmsClientOptions,
            _observabilityOptions,
            _dlmsClientLogger,
            _logger,
            _timeProvider,
            _dlmsClientService);
    }
    
    [Fact]
    public async Task Register_InvalidClientId_ThrowsRpcException()
    {
        // Arrange
        var request = new AvailabilityRequest
        {
            ClientId = "invalid-client-id",
        };
        
        // Act
        Func<Task> act = async () => await _meterAvailabilityService.Register(request, Substitute.For<ServerCallContext>());
        
        // Assert
        await act.Should().ThrowAsync<RpcException>();
    }
}