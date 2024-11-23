using System.Diagnostics;
using Electrify.Dlms.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Electrify.Dlms.UnitTests.Extensions;

public class LoggerExtensionsTests
{
    private readonly ILogger _logger;
    
    public LoggerExtensionsTests()
    {
        _logger = Substitute.For<ILogger>();
    }
    
    [Fact]
    public void CurrentLogLevel_Should_Return_Highest_Enabled_LogLevel()
    {
        // Arrange
        _logger.IsEnabled(LogLevel.Trace).Returns(false);
        _logger.IsEnabled(LogLevel.Debug).Returns(false);
        _logger.IsEnabled(LogLevel.Information).Returns(false);
        _logger.IsEnabled(LogLevel.Warning).Returns(false);
        _logger.IsEnabled(LogLevel.Error).Returns(true);
        _logger.IsEnabled(LogLevel.Critical).Returns(false);
        _logger.IsEnabled(LogLevel.None).Returns(false);

        // Act
        var logLevel = _logger.CurrentLogLevel();

        // Assert
        logLevel.Should().Be(LogLevel.Error);
    }

    [Theory]
    [InlineData(LogLevel.Trace, TraceLevel.Verbose)]
    [InlineData(LogLevel.Debug, TraceLevel.Verbose)]
    [InlineData(LogLevel.Information, TraceLevel.Info)]
    [InlineData(LogLevel.Warning, TraceLevel.Warning)]
    [InlineData(LogLevel.Error, TraceLevel.Error)]
    [InlineData(LogLevel.Critical, TraceLevel.Error)]
    [InlineData(LogLevel.None, TraceLevel.Off)]
    public void ToTraceLevel_Should_Return_Correlated_TraceLevel(LogLevel logLevel, TraceLevel traceLevel)
    {
        // Act
        var level = logLevel.ToTraceLevel();
        
        // Assert
        level.Should().Be(traceLevel);
    }

    [Fact]
    public void ToTraceLevel_Should_Throw_UnreachableException_If_Enum_Is_Invalid()
    {
        // Arrange
        var logLevel = (LogLevel)9999;
        
        // Act
        Action act = () => logLevel.ToTraceLevel();
        
        // Assert
        Assert.Throws<UnreachableException>(act);
    }
}