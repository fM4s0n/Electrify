using Electrify.AdminUi.Services;
using Electrify.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Time.Testing;

namespace Electrify.AdminUi.UnitTests.Services;

public class GreetingServiceTests
{
    private readonly FakeTimeProvider _fakeTimeProvider;

    public GreetingServiceTests()
    {
        _fakeTimeProvider = new FakeTimeProvider();
        _fakeTimeProvider.SetLocalTimeZone(TimeZoneInfo.Utc);
    }

    [Theory]
    [InlineData(0, "Good Morning")]
    [InlineData(1, "Good Morning")]
    [InlineData(13, "Good Afternoon")]
    [InlineData(16, "Good Afternoon")]
    [InlineData(18, "Good Evening")]
    [InlineData(23, "Good Evening")]
    public void GetGreeting_Should_Return_Correct_Greeting(int hour, string expectedGreeting)
    {
        // Arrange
        var date = new DateTime(2024, 1, 1, hour, 1, 0);
        _fakeTimeProvider.SetUtcNow(date);     

        var greetingService = new GreetingService(_fakeTimeProvider);
        var admin = new Admin
        {
            Name = "Admin",
            Id = Guid.NewGuid(),
            Email = string.Empty,
            PasswordHash = string.Empty,
        };

        // Act
        var greeting = greetingService.GetGreeting(admin.Name);

        // Assert
        using (new AssertionScope())
        {
            greeting.Should().Be($"{expectedGreeting}, {admin.Name}");
        }
    }
}
