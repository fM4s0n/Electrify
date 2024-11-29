using Electrify.AdminUi.Services;
using Electrify.Server.ApiClient.Abstraction;
using FluentAssertions;
using NSubstitute;

namespace Electrify.AdminUi.UnitTests.Services
{
    public class BillServiceTests
    {
        private readonly IElectrifyApiClient _apiClient = Substitute.For<IElectrifyApiClient>();
        private readonly BillService _billService;

        public BillServiceTests()
        {
            _billService = new BillService(_apiClient);
        }

        [Fact]
        public async Task GetBillForDay_ReturnsBillAmount()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var date = new DateOnly(2023, 10, 1);
            var expectedBill = 100.50;
            _apiClient.GetClientBill(clientId.ToString(), date).Returns(expectedBill);

            // Act
            var result = await _billService.GetBillForDay(clientId, date);

            // Assert
            result.Should().Be(expectedBill);
        }

        [Fact]
        public async Task GetBillForDay_ReturnsNull_WhenApiReturnsNull()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var date = new DateOnly(2023, 10, 1);
            _apiClient.GetClientBill(clientId.ToString(), date).Returns((double?)null);

            // Act
            var result = await _billService.GetBillForDay(clientId, date);

            // Assert
            result.Should().BeNull();
        }
    }
}