using Electrify.Models;
using FluentAssertions;

namespace Electrify.ComponentTests.Services;

public class GetClientBillTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    [Fact]
    public async Task GetClientBill_Should_Return_Bill_For_Client()
    {
        // Arrange
        Guid clientId = Guid.NewGuid();
        DateTime date = DateTime.Now;

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

        await fixture.Database.Readings.AddRangeAsync(readings);
        await fixture.Database.SaveChangesAsync();
        
        // Act
        var result = await fixture.ApiClient.GetClientBill(clientId.ToString(), DateOnly.FromDateTime(date));
        
        // Assert
        result.Should().Be(10);
    }
    
    [Fact]
    public async Task GetClientBill_Should_Return_Zero_If_No_Readings()
    {
        // Arrange
        Guid clientId = Guid.NewGuid();
        DateTime date = DateTime.Now;
        
        // Act
        var result = await fixture.ApiClient.GetClientBill(clientId.ToString(), DateOnly.FromDateTime(date));
        
        // Assert
        result.Should().Be(0.0);
    }
}