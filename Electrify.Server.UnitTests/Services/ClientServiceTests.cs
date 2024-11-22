using Electrify.Models;
using Electrify.Server.Database;
using Electrify.Server.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Electrify.Server.UnitTests.Services;

public class ClientServiceTests
{
    private readonly ClientService _clientService;
    private readonly ILogger<ClientService> _logger;
    private readonly ElectrifyDbContext _database;
    
    public ClientServiceTests()
    {
        var builder = new DbContextOptionsBuilder<ElectrifyDbContext>();
        builder.UseInMemoryDatabase("UnitTestDb");
        _database = new ElectrifyDbContext(builder.Options);
        _logger = NSubstitute.Substitute.For<ILogger<ClientService>>();

        _clientService = new ClientService(_database, _logger);
    }

    [Fact]
    public async Task ClientExists_Should_Return_True_When_Client_In_Database()
    {
        // Arrange
        var client = new Client
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
        };

        _database.Clients.Add(client);
        await _database.SaveChangesAsync();
        
        // Act
        var result = await _clientService.ClientExists(client.UserId, client.Id);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task ClientExists_Should_Return_False_When_Client_NotIn_Database()
    {
        // Arrange
        var client = new Client
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
        };
        
        // Act
        var result = await _clientService.ClientExists(client.UserId, client.Id);
        
        // Assert
        result.Should().BeFalse();
    }
}