using Electrify.Models.Models;
using Electrify.Server.Database;
using Electrify.Server.Services;
using Electrify.Server.Services.Abstraction;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Electrify.UnitTests.Server.Services;

public class ClientServiceTests
{
    private readonly IClientService _clientService;
    private readonly ElectrifyDbContext _database;
    
    public ClientServiceTests()
    {
        var builder = new DbContextOptionsBuilder<ElectrifyDbContext>();
        builder.UseInMemoryDatabase("UnitTestDb");
        _database = new ElectrifyDbContext(builder.Options);
        _clientService = new ClientService(_database);
    }

    [Fact]
    public async Task ClientExists_Should_Return_True_When_Client_In_Database()
    {
        // Arrange
        var client = new Client
        {
            UserId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
        };

        _database.Clients.Add(client);
        await _database.SaveChangesAsync();
        
        // Act
        var result = await _clientService.ClientExists(client.UserId, client.ClientId);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task ClientExists_Should_Return_False_When_Client_NotIn_Database()
    {
        // Arrange
        var client = new Client
        {
            UserId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
        };
        
        // Act
        var result = await _clientService.ClientExists(client.UserId, client.ClientId);
        
        // Assert
        result.Should().BeFalse();
    }
}