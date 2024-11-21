
using Electrify.AdminUi.Services;
using Electrify.Models;
using Electrify.Server.ApiClient.Abstraction;
using Electrify.Server.ApiClient.Contracts;
using FluentAssertions;
using NSubstitute;

namespace Electrify.AdminUi.UnitTests.Services;

public class ClientServiceTests
{
    private readonly IElectrifyApiClient _electrifyApiClient;

    public ClientServiceTests()
    {
        _electrifyApiClient = Substitute.For<IElectrifyApiClient>();
    }

    [Fact]
    public async Task InsertClient_Should_Return_True_When_Response_IsTrue()
    {
        // Arrange
        _electrifyApiClient.InsertClient(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns(new HttpInsertClientResponse { Success = true });

        var clientService = new ClientService(_electrifyApiClient);
        var newClient = new Client
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act
        var result = await clientService.InsertClient(newClient);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task InsertClient_Should_Return_False_When_Response_IsFalse()
    {
        // Arrange
        _electrifyApiClient.InsertClient(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns(new HttpInsertClientResponse { Success = false });

        var clientService = new ClientService(_electrifyApiClient);
        var newClient = new Client
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act
        var result = await clientService.InsertClient(newClient);

        // Assert
        result.Should().BeFalse();
    }
}
