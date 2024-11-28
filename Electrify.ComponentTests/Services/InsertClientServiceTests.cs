using FluentAssertions;

namespace Electrify.ComponentTests.Services;

public sealed class InsertClientServiceTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    [Fact]
    public async Task InsertClient_Should_Create_New_Client_In_Database_And_Return_Success()
    {
        // Arrange
        var token = fixture.Database.Admins.First().AccessToken.ToString()!;
        var clientId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        // Act
        var response = await fixture.ApiClient.InsertClient(token, clientId, userId);
        
        // Assert
        response.Success.Should().BeTrue();

        var client = fixture.Database.Clients
            .SingleOrDefault(c => c.Id == clientId && c.UserId == userId);
        
        client.Should().NotBeNull();
    }
}