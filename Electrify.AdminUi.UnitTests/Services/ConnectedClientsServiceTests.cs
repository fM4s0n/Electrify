using Electrify.AdminUi.Services;
using Electrify.AdminUi.Services.Abstractions;
using Electrify.Server.ApiClient.Abstraction;
using Electrify.Server.ApiClient.Contracts;
using FluentAssertions;
using NSubstitute;

namespace Electrify.AdminUi.UnitTests.Services
{
    public class ConnectedClientsServiceTests
    {
        private readonly IElectrifyApiClient _mockApiClient;
        private readonly IAdminService _mockAdminService;
        private readonly ConnectedClientsService _connectedClientsService;

        public ConnectedClientsServiceTests()
        {
            _mockApiClient = Substitute.For<IElectrifyApiClient>();
            _mockAdminService = Substitute.For<IAdminService>();
            _connectedClientsService = new ConnectedClientsService(_mockApiClient, _mockAdminService);
        }

        [Fact]
        public async Task GetConnectedClientIds_ReturnsClientIds_WhenAdminIsNotNull()
        {
            // Arrange
            var expectedClientIds = new List<string> { "client1", "client2" };
            var adminToken = "admin-token";
            _mockAdminService.CurrentAdmin
                .Returns(new HttpAdminLoginResponse() { Id = Guid.NewGuid().ToString(), Email = "", Name = "", Success = true, Token = adminToken });
            _mockApiClient.GetConnectedClientIds(adminToken).Returns(expectedClientIds);

            // Act
            var result = await _connectedClientsService.GetConnectedClientIds();

            // Assert
            result.Should().BeEquivalentTo(expectedClientIds);
        }

        [Fact]
        public async Task GetConnectedClientIds_ReturnsEmptyList_WhenAdminIsNull()
        {
            // Arrange
            _mockAdminService.CurrentAdmin.Returns((HttpAdminLoginResponse?)null);

            // Act
            var result = await _connectedClientsService.GetConnectedClientIds();

            // Assert
            result.Should().BeEmpty();
        }
    }
}