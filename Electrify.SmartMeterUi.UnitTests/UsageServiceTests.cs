using Electrify.Dlms.Server.Abstraction;
using Electrify.SmartMeterUi.Services;
using FluentAssertions;
using NSubstitute;

namespace Electrify.SmartMeterUi.UnitTests;

public class UsageServiceTests
{
    [Fact]
    public void GetCumulativeUsage_ShouldReturnInitialUsage()
    {
        // Arrange
        var dlmsServerMock = Substitute.For<IDlmsServer>();
        var usageService = new UsageService(dlmsServerMock);

        // Act
        var result = usageService.GetCumulativeUsage();

        // Assert
        result.Usage.Should().BeGreaterThan(0);
        result.TimeStamp.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddNewReading_ShouldUpdateUsage()
    {
        // Arrange
        var dlmsServerMock = Substitute.For<IDlmsServer>();
        var usageService = new UsageService(dlmsServerMock);

        // Act
        usageService.GetCumulativeUsage(); // Initial reading
        Thread.Sleep(1100); // Wait for the timer to elapse
        var result = usageService.GetCumulativeUsage();

        // Assert
        result.Usage.Should().BeGreaterThan(0f);
        dlmsServerMock.Received().SetEnergy(Arg.Any<double>());
    }
    
    [Fact]
    public void GetCurrentUsage_ShouldReturnCurrentUsage()
    {
        // Arrange
        var dlmsServerMock = Substitute.For<IDlmsServer>();
        var usageService = new UsageService(dlmsServerMock);

        // Act
        usageService.GetCumulativeUsage(); // Initial reading
        Thread.Sleep(1100); // Wait for the timer to elapse
        var result = usageService.GetCurrentUsage();

        // Assert
        result.Should().BeGreaterThan(0f);
    }

    [Fact]
    public void Dispose_ShouldDisposeTimer()
    {
        // Arrange
        var dlmsServerMock = Substitute.For<IDlmsServer>();
        var usageService = new UsageService(dlmsServerMock);

        // Act
        usageService.Dispose();

        // Assert
        usageService.Invoking(us => us.Dispose()).Should().NotThrow();
    }
}