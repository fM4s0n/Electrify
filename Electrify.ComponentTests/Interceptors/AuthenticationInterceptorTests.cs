using Electrify.Server.ApiClient.Contracts;
using FluentAssertions;

namespace Electrify.ComponentTests.Interceptors;

public class AuthenticationInterceptorTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    [Fact]
    public async Task InsertClient_Should_Throw_With_Invalid_Token()
    {
        // Arrange
        var token = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        const string expectedResponse = "{\"code\":16,\"message\":\"Token is invalid\",\"details\":[]}";
        
        // Act
        HttpInsertClientResponse? response = null;
        Exception? exception = null;
        try
        {
            response = await fixture.ApiClient.InsertClient(token.ToString(), clientId, userId);
        }
        catch (Exception ex)
        {
            exception = ex;
        }
        
        // Assert
        response.Should().BeNull();
        exception.Should().NotBeNull();
        exception!.Message.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task InsertClient_Should_Throw_With_Invalid_Formatted_Token()
    {
        // Arrange
        const string token = "not-a-valid-guid";
        var clientId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        const string expectedResponse = "{\"code\":3,\"message\":\"Token must be in GUID format\",\"details\":[]}";
        
        // Act
        HttpInsertClientResponse? response = null;
        Exception? exception = null;
        try
        {
            response = await fixture.ApiClient.InsertClient(token, clientId, userId);
        }
        catch (Exception ex)
        {
            exception = ex;
        }
        
        // Assert
        response.Should().BeNull();
        exception.Should().NotBeNull();
        exception!.Message.Should().BeEquivalentTo(expectedResponse);
    }
}