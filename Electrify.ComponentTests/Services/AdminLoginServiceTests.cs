using FluentAssertions;
using FluentAssertions.Execution;

namespace Electrify.ComponentTests.Services;

public sealed class AdminLoginServiceTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    [Fact]
    public async Task AdminLogin_Returns_Success_With_User_Details_If_Login_Correct()
    {
        // Arrange
        const string email = "admin@electrify.com";
        const string password = "Password";
        
        // Act
        var response = await fixture.ApiClient.AdminLogin(email, password);
        
        // Assert
        using (new AssertionScope())
        {
            response.Success.Should().BeTrue();
            response.Id.Should().NotBeEmpty();
            response.Name.Should().Be("Administrator");
            response.Email.Should().Be(email);
            response.PasswordHash.Should().NotBeEmpty();  // TODO we shouldn't be returning the password hash it's insecure
            response.Token.Should().NotBeEmpty();
        }
    }
    
    [Fact]
    public async Task AdminLogin_Returns_Unsuccessful_If_Login_Incorrect()
    {
        // Arrange
        var email = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        
        // Act
        var response = await fixture.ApiClient.AdminLogin(email, password);
        
        // Assert
        response.Success.Should().BeFalse();
    }
}