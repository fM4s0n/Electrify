using Electrify.Models;
using Electrify.Server.Database;
using Electrify.Server.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Serilog;

namespace Electrify.Server.UnitTests.Services;

public class ReadingServiceTests
{
    private readonly ElectrifyDbContext _database;
    private readonly ILogger<ReadingService> _logger = Substitute.For<ILogger<ReadingService>>();
    private readonly ReadingService _readingService;
    
    public ReadingServiceTests()
    {
        var builder = new DbContextOptionsBuilder<ElectrifyDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
        _database = new ElectrifyDbContext(builder.Options);
        
        _readingService = new ReadingService(_database, _logger);
    }
    
    [Fact]
    public void GetReadingsForDay_Should_Return_Zero_When_No_Readings()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Now);
        
        // Act
        var result = _readingService.GetReadingsForDay(clientId, date);
        
        // Assert
        result.Should().Be(0.0);
    }
    
    [Fact]
    public void GetReadingsForDay_Should_Return_Correct_Cost()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Now);
        
        List<Reading> readings =
        [
            new Reading
            {
                ClientId = clientId,
                DateTime = DateTime.Now.AddDays(-1),
                EnergyUsage = 100,
                Tariff = 0.1
            },
            new Reading
            {
                ClientId = clientId,
                DateTime = DateTime.Now,
                EnergyUsage = 200,
                Tariff = 0.1
            }
        ];
        
        _database.Readings.AddRange(readings);
        _database.SaveChanges();
        
        // Act
        var result = _readingService.GetReadingsForDay(clientId, date);
        
        // Assert
        result.Should().Be(10.0);
    }
    
    [Fact]
    public void GetReadingsForDay_Should_Return_Correct_Cost_When_Multiple_Readings()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Now);
        
        List<Reading> readings =
        [
            new Reading
            {
                ClientId = clientId,
                DateTime = DateTime.Now.AddDays(-2),
                EnergyUsage = 100,
                Tariff = 0.1
            },
            new Reading
            {
                ClientId = clientId,
                DateTime = DateTime.Now.AddDays(-1),
                EnergyUsage = 200,
                Tariff = 0.1
            },
            new Reading
            {
                ClientId = clientId,
                DateTime = DateTime.Now,
                EnergyUsage = 300,
                Tariff = 0.1
            }
        ];
        
        _database.Readings.AddRange(readings);
        _database.SaveChanges();
        
        // Act
        var result = _readingService.GetReadingsForDay(clientId, date);
        
        // Assert
        result.Should().Be(10.0);
    }
}