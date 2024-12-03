using Electrify.Protos;
using Electrify.Server.Services;
using Electrify.Server.Services.Abstraction;
using FluentAssertions;
using Grpc.Core;
using NSubstitute;

namespace Electrify.Server.UnitTests.Services;

public class ConnectedClientsServiceTests
{
    private readonly IAdminService _adminService = Substitute.For<IAdminService>();
    private readonly IDlmsClientService _dlmsClientService = Substitute.For<IDlmsClientService>();
    
    private readonly ConnectedClientsService _connectedClientsService;
    
    public ConnectedClientsServiceTests()
    {
        _connectedClientsService = new ConnectedClientsService(_adminService, _dlmsClientService);
    }
    
    [Fact]
    public async Task GetConnectedIds_Should_Return_ClientIds()
    {
        // Arrange
        Guid token = Guid.NewGuid();
        
        var request = new GetConnectedClientIdsRequest
        {
            Token = token.ToString()
        };
        
        _adminService.ValidateToken(token).Returns(true);
        
        var context = Substitute.For<ServerCallContext>();
        
        // Act
        var response = await _connectedClientsService.GetConnectedClientIds(request, context);
        
        // Assert
        response.Should().NotBeNull();
    }
}