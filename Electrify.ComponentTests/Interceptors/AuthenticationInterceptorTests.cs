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
        exception!.Message.Should().BeEquivalentTo("{\"code\":16,\"message\":\"Token is invalid\",\"details\":[]}");
    }
}