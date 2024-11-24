using Electrify.Protos;
using Electrify.Server.Services;
using Electrify.Server.Services.Abstraction;
using FluentAssertions;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Electrify.Server.UnitTests.Services;

public class AuthenticationServiceTests
{
    private readonly IClientService _clientService = Substitute.For<IClientService>();
    
    private readonly AuthenticationService _authenticationService;

    public AuthenticationServiceTests()
    {
        _authenticationService = new AuthenticationService(_clientService, Substitute.For<ILogger<AuthenticationService>>());
    }

    [Fact]
    public async Task Authenticate_InvalidUserId_ThrowsRpcException()
    {
        // Arrange
        var request = new AuthenticateRequest
        {
            UserId = "invalid",
            ClientId = Guid.NewGuid().ToString()
        };

        var context = Substitute.For<ServerCallContext>();

        // Act
        Func<Task> act = async () => await _authenticationService.Authenticate(request, context);
        
        // Assert
        await act.Should().ThrowAsync<RpcException>();
    }
    
    [Fact]
    public async Task Authenticate_InvalidClientId_ThrowsRpcException()
    {
        // Arrange
        var request = new AuthenticateRequest
        {
            UserId = Guid.NewGuid().ToString(),
            ClientId = "invalid"
        };

        var context = Substitute.For<ServerCallContext>();

        // Act
        Func<Task> act = async () => await _authenticationService.Authenticate(request, context);
        
        // Assert
        await act.Should().ThrowAsync<RpcException>();
    }
    
    [Fact]
    public async Task Authenticate_ClientDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var request = new AuthenticateRequest
        {
            UserId = Guid.NewGuid().ToString(),
            ClientId = Guid.NewGuid().ToString()
        };

        var context = Substitute.For<ServerCallContext>();

        _clientService.ClientExists(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(false);

        // Act
        var response = await _authenticationService.Authenticate(request, context);
        
        // Assert
        response.Success.Should().BeFalse();
    }
    
    [Fact]
    public async Task Authenticate_ClientExists_ReturnsTrue()
    {
        // Arrange
        var request = new AuthenticateRequest
        {
            UserId = Guid.NewGuid().ToString(),
            ClientId = Guid.NewGuid().ToString()
        };

        var context = Substitute.For<ServerCallContext>();

        _clientService.ClientExists(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);

        // Act
        var response = await _authenticationService.Authenticate(request, context);
        
        // Assert
        response.Success.Should().BeTrue();
    }
}