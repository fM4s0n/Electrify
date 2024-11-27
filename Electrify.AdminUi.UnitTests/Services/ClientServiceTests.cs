
using Electrify.AdminUi.Services;
using Electrify.AdminUi.Services.Abstractions;
using Electrify.Models;
using Electrify.Server.ApiClient.Abstraction;
using Electrify.Server.ApiClient.Contracts;
using FluentAssertions;
using NSubstitute;

namespace Electrify.AdminUi.UnitTests.Services;

public class ClientServiceTests
{
    private readonly IElectrifyApiClient _electrifyApiClient;
    private readonly ClientService _clientService;
    private readonly HttpAdminLoginResponse _admin;

    public ClientServiceTests()
    {
        _admin = new HttpAdminLoginResponse
        {
            Success = true,
            Id = Guid.NewGuid().ToString(),
            Name = Guid.NewGuid().ToString(),
            Email = "unit@test.com",
            Token = Guid.NewGuid().ToString()
        };

        var adminService = Substitute.For<IAdminService>();

        adminService.CurrentAdmin.Returns(_admin);
        
        _electrifyApiClient = Substitute.For<IElectrifyApiClient>();
        _clientService = new ClientService(_electrifyApiClient, adminService);
    }

    [Fact]
    public async Task InsertClient_Should_Return_True_When_Response_IsTrue()
    {
        // Arrange
        var newClient = new Client
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };
        
        _electrifyApiClient.InsertClient(_admin.Token, newClient.Id, newClient.UserId)
            .Returns(new HttpInsertClientResponse { Success = true });

        // Act
        var result = await _clientService.InsertClient(newClient);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task InsertClient_Should_Return_False_When_Response_IsFalse()
    {
        // Arrange
        var newClient = new Client
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };
        
        _electrifyApiClient.InsertClient(_admin.Token, newClient.Id, newClient.UserId)
            .Returns(new HttpInsertClientResponse { Success = false });

        // Act
        var result = await _clientService.InsertClient(newClient);

        // Assert
        result.Should().BeFalse();
    }
}
