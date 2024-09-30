using Electrify.Server.Models;
using Electrify.Server.Services.Abstraction;

namespace Electrify.Server.Services;

public class OctopusService(IHttpClientFactory clientFactory) : IOctopusService
{
    private readonly HttpClient _client = clientFactory.CreateClient("OctopusClient");

    public async Task<double?> GetPrice(DateTimeOffset readingTime)
    {
        string periodFrom = readingTime.AddMinutes(-1).ToString("yyyy-MM-ddTHH:mm");
        string periodTo = readingTime.ToString("yyyy-MM-ddTHH:mm");
        
        HttpResponseMessage response = await _client
            .GetAsync($"/v1/products/AGILE-18-02-21/electricity-tariffs/E-1R-AGILE-18-02-21-C/standard-unit-rates?period_from={periodFrom}&period_to={periodTo}");

        if (response.IsSuccessStatusCode == false)
        {
            return null;
        }

        OctopusResponse? octopusResponse = await response.Content.ReadFromJsonAsync<OctopusResponse>();

        if (octopusResponse == null || octopusResponse.Results.Any() == false)
        {
            return null;
        }
        
        double value = octopusResponse.Results[0].ValueExcludingVat;

        // Add 12p if reading time is between 4am and 7am
        if (readingTime.Hour >= 4 && readingTime.Hour <= 7)
        {
            value += 12;
        }

        // Add VAT
        value = value * 1.05;
        
        // Take into account price cap
        return value >= 33.33 ? 33.33 : value;
    }


    public async Task<Dictionary<int, double>> GetDailyPrices(DateTimeOffset date)
    {
        Dictionary<int, double> prices = new();
        
        // Stop at 23 as there's no price for 23:30
        for (int i = 0; i < 24; i++)
        {
            DateTimeOffset priceHour = new DateTimeOffset(
                date.Year,
                date.Month,
                date.Day,
                i,
                2,
                0, // seconds
                date.Offset // Keep the same offset
            );
            
            double? price = await GetPrice(priceHour);

            if (price == null)
            {
                continue;
            }
            
            prices.Add(i, price.Value);
        }

        return prices;
    }
}