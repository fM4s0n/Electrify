using Electrify.SmartMeterUi.Services;
using FluentAssertions;

namespace Electrify.SmartMeterUi.UnitTests;

public class ErrorMessageServiceTests
{
    private readonly ErrorMessageService _service;

    public ErrorMessageServiceTests()
    {
        _service = new ErrorMessageService();
    }
    
    [Fact]
    public void IsConnected_Can_Be_Set_And_Get()
    {
        _service.IsConnected.Should().BeFalse();
        _service.IsConnected = true;
        _service.IsConnected.Should().BeTrue();
    }
    
    [Fact]
    public void ErrorMessage_Can_Be_Set_And_Get()
    {
        var message = Guid.NewGuid().ToString();
        
        _service.ErrorMessage.Should().BeNull();
        _service.ErrorMessage = message;
        _service.ErrorMessage.Should().Be(message);
    }
}