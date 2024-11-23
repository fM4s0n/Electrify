using System.Net;
using System.Text;
using System.Text.Json;
using Electrify.Server.Models;
using Electrify.Server.Services;
using Electrify.Server.UnitTests.Substitutes;
using FluentAssertions;
using NSubstitute;

namespace Electrify.Server.UnitTests.Services;

public class OctopusServiceTests
{ 
    [Fact]
    public async Task GetPrice_ReturnsExpectedPrice_WhenBetween4And7AM()
    {
        // Arrange
        var readingTime = new DateTimeOffset(2023, 10, 1, 5, 0, 0, TimeSpan.Zero); // 5 AM

        var responseContent = new OctopusResponse
        {
            Results =
            [
                new Results
                {
                    ValueExcludingVat = 20,
                    ValidFrom = readingTime.AddMinutes(-30).ToString("o")
                }
            ]
        };

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseContent), Encoding.UTF8, "application/json")
        };

        var mockHandler = new SubstituteHttpMessageHandler(_ => Task.FromResult(responseMessage));

        var client = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://test.com")
        };

        var clientFactory = Substitute.For<IHttpClientFactory>();
        clientFactory.CreateClient("OctopusClient").Returns(client);

        var service = new OctopusService(clientFactory);

        // Act
        var price = await service.GetPrice(readingTime);

        // Assert
        price.Should().Be(33.33);
    }

    // Test when HTTP response is unsuccessful
    [Fact]
    public async Task GetPrice_ReturnsNull_WhenResponseIsUnsuccessful()
    {
        // Arrange
        var readingTime = new DateTimeOffset(2023, 10, 1, 12, 0, 0, TimeSpan.Zero); // 12 PM

        var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        var mockHandler = new SubstituteHttpMessageHandler(_ => Task.FromResult(responseMessage));

        var client = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://test.com")
        };

        var clientFactory = Substitute.For<IHttpClientFactory>();
        clientFactory.CreateClient("OctopusClient").Returns(client);

        var service = new OctopusService(clientFactory);

        // Act
        var price = await service.GetPrice(readingTime);

        // Assert
        price.Should().BeNull();
    }

    // Test when results are empty
    [Fact]
    public async Task GetPrice_ReturnsNull_WhenResultsAreEmpty()
    {
        // Arrange
        var readingTime = new DateTimeOffset(2023, 10, 1, 12, 0, 0, TimeSpan.Zero); // 12 PM

        var responseContent = new OctopusResponse
        {
            Results = []
        };

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseContent), Encoding.UTF8, "application/json")
        };

        var mockHandler = new SubstituteHttpMessageHandler(_ => Task.FromResult(responseMessage));

        var client = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://test.com")
        };

        var clientFactory = Substitute.For<IHttpClientFactory>();
        clientFactory.CreateClient("OctopusClient").Returns(client);

        var service = new OctopusService(clientFactory);

        // Act
        var price = await service.GetPrice(readingTime);

        // Assert
        price.Should().BeNull();
    }

    // Test price calculation outside 4 AM to 7 AM
    [Fact]
    public async Task GetPrice_CalculatesPriceCorrectly_WhenOutside4And7AM()
    {
        // Arrange
        var readingTime = new DateTimeOffset(2023, 10, 1, 12, 0, 0, TimeSpan.Zero); // 12 PM
        var valueExcludingVat = 20.0; // 20p

        var responseContent = new OctopusResponse
        {
            Results =
            [
                new Results
                {
                    ValueExcludingVat = valueExcludingVat,
                    ValidFrom = readingTime.AddMinutes(-30).ToString("o")
                }
            ]
        };

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseContent), Encoding.UTF8, "application/json")
        };

        var mockHandler = new SubstituteHttpMessageHandler(_ => Task.FromResult(responseMessage));

        var client = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://test.com")
        };

        var clientFactory = Substitute.For<IHttpClientFactory>();
        clientFactory.CreateClient("OctopusClient").Returns(client);

        var service = new OctopusService(clientFactory);

        // Act
        var price = await service.GetPrice(readingTime);

        // Calculate expected price
        var expectedPrice = Math.Round(20 * 1.05, 2); // Should be 21.00

        // Assert
        price.Should().Be(expectedPrice);
    }

    // Test price cap application
    [Fact]
    public async Task GetPrice_ReturnsCappedPrice_WhenPriceExceedsCap()
    {
        // Arrange
        var readingTime = new DateTimeOffset(2023, 10, 1, 12, 0, 0, TimeSpan.Zero); // 12 PM
        var valueExcludingVat = 40.0; // 40p

        var responseContent = new OctopusResponse
        {
            Results =
            [
                new Results
                {
                    ValueExcludingVat = valueExcludingVat,
                    ValidFrom = readingTime.AddMinutes(-30).ToString("o")
                }
            ]
        };

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseContent), Encoding.UTF8, "application/json")
        };

        var mockHandler = new SubstituteHttpMessageHandler(_ => Task.FromResult(responseMessage));

        var client = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://test.com")
        };

        var clientFactory = Substitute.For<IHttpClientFactory>();
        clientFactory.CreateClient("OctopusClient").Returns(client);

        var service = new OctopusService(clientFactory);

        // Act
        var price = await service.GetPrice(readingTime);

        // Assert
        price.Should().Be(33.33);
    }

    // Test successful retrieval of daily prices
    [Fact]
    public async Task GetDailyPrices_ReturnsExpectedPrices()
    {
        // Arrange
        var date = new DateTimeOffset(2023, 10, 1, 0, 0, 0, TimeSpan.Zero);

        var results = new List<Results>();
        for (int hour = 0; hour < 24; hour++)
        {
            results.Add(new Results
            {
                ValueExcludingVat = 1.0,
                ValidFrom = date.AddHours(hour).ToString("o")
            });
            
            results.Add(new Results
            {
                ValueExcludingVat = 1.0,
                ValidFrom = date.AddHours(hour).AddMinutes(30).ToString("o")
            }
            );
        }
        
        var responseContent = new OctopusResponse
        {
            Results = results.ToArray()
        };

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseContent), Encoding.UTF8, "application/json")
        };

        var mockHandler = new SubstituteHttpMessageHandler(_ => Task.FromResult(responseMessage));

        var client = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://test.com")
        };

        var clientFactory = Substitute.For<IHttpClientFactory>();
        clientFactory.CreateClient("OctopusClient").Returns(client);

        var service = new OctopusService(clientFactory);

        // Act
        var prices = await service.GetDailyPrices(date);

        // Assert
        prices.Should().NotBeNull();
        prices!.Count.Should().Be(24);
        
        Dictionary<int, double> expectedPrices = new Dictionary<int, double>
        {
            [0] = 1.05,
            [1] = 1.05,
            [2] = 1.05,
            [3] = 1.05,
            [4] = 13.65,
            [5] = 13.65,
            [6] = 13.65,
            [7] = 13.65,
            [8] = 1.05,
            [9] = 1.05,
            [10] = 1.05,
            [11] = 1.05,
            [12] = 1.05,
            [13] = 1.05,
            [14] = 1.05,
            [15] = 1.05,
            [16] = 1.05,
            [17] = 1.05,
            [18] = 1.05,
            [19] = 1.05,
            [20] = 1.05,
            [21] = 1.05,
            [22] = 1.05,
            [23] = 1.05
        };
        
        prices.Should().BeEquivalentTo(expectedPrices);
    }

    // Test when HTTP response is unsuccessful
    [Fact]
    public async Task GetDailyPrices_ReturnsNull_WhenResponseIsUnsuccessful()
    {
        // Arrange
        var date = new DateTimeOffset(2023, 10, 1, 0, 0, 0, TimeSpan.Zero);

        var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        var mockHandler = new SubstituteHttpMessageHandler(_ => Task.FromResult(responseMessage));

        var client = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://test.com")
        };

        var clientFactory = Substitute.For<IHttpClientFactory>();
        clientFactory.CreateClient("OctopusClient").Returns(client);

        var service = new OctopusService(clientFactory);

        // Act
        var prices = await service.GetDailyPrices(date);

        // Assert
        prices.Should().BeNull();
    }

    // Test when results are empty
    [Fact]
    public async Task GetDailyPrices_ReturnsEmptyDictionary_WhenResultsAreEmpty()
    {
        // Arrange
        var date = new DateTimeOffset(2023, 10, 1, 0, 0, 0, TimeSpan.Zero);

        var responseContent = new OctopusResponse
        {
            Results = []
        };

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseContent), Encoding.UTF8, "application/json")
        };

        var mockHandler = new SubstituteHttpMessageHandler(_ => Task.FromResult(responseMessage));

        var client = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://test.com")
        };

        var clientFactory = Substitute.For<IHttpClientFactory>();
        clientFactory.CreateClient("OctopusClient").Returns(client);
        
        var sut = new OctopusService(clientFactory);

        // Act
        var prices = await sut.GetDailyPrices(date);

        // Assert
        prices.Should().BeNull();
    }

    // Test handling of null ValidFrom
    [Fact]
    public async Task GetDailyPrices_SkipsResultsWithNullValidFrom()
    {
        // Arrange
        var date = new DateTimeOffset(2023, 10, 1, 0, 0, 0, TimeSpan.Zero);

        var results = new[]
        {
            new Results
            {
                ValueExcludingVat = 10.0,
                ValidFrom = null
            },
            new Results
            {
                ValueExcludingVat = 15.0,
                ValidFrom = date.AddHours(1).ToString("o")
            }
        };

        var responseContent = new OctopusResponse
        {
            Results = results
        };

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(responseContent), Encoding.UTF8, "application/json")
        };

        var mockHandler = new SubstituteHttpMessageHandler(_ => Task.FromResult(responseMessage));

        var client = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://test.com")
        };

        var clientFactory = Substitute.For<IHttpClientFactory>();
        clientFactory.CreateClient("OctopusClient").Returns(client);

        var service = new OctopusService(clientFactory);

        // Act
        var prices = await service.GetDailyPrices(date);

        // Assert
        prices.Should().NotBeNull();
        prices!.Count.Should().Be(1);
        prices.Should().ContainKey(1);
    }
}

