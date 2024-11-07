using Electrify.Server.Models;
using Electrify.Server.Services.Abstraction;
using Results = Electrify.Server.Models.Results;

namespace Electrify.Server.Services;

public class OctopusService(IHttpClientFactory clientFactory) : IOctopusService
{
    private readonly HttpClient _client = clientFactory.CreateClient("OctopusClient");

    public async Task<double?> GetPrice(DateTimeOffset readingTime)
    {
        string periodFrom = readingTime.AddMinutes(-1).ToString("yyyy-MM-ddTHH:mm");
        string periodTo = readingTime.ToString("yyyy-MM-ddTHH:mm");

        OctopusResponse? octopusResponse = await GetOctopusResponse(periodFrom, periodTo);
        
        return octopusResponse != null 
            ? GetFinalPrice(octopusResponse.Results[0].ValueExcludingVat, readingTime.Hour) 
            : null;
    }

    private async Task<OctopusResponse?> GetOctopusResponse(string periodFrom, string periodTo)
    {
        HttpResponseMessage response = await _client
            .GetAsync($"/v1/products/AGILE-18-02-21/electricity-tariffs/E-1R-AGILE-18-02-21-C/standard-unit-rates?period_from={periodFrom}&period_to={periodTo}");

        if (response.IsSuccessStatusCode == false)
        {
            return null;
        }

        OctopusResponse? octopusResponse = await response.Content.ReadFromJsonAsync<OctopusResponse>();
        
        return octopusResponse == null || octopusResponse.Results.Any() == false ? null : octopusResponse;
    }

    private static double GetFinalPrice(double price, int hour)
    {
        // Add 12p if reading time is between 4am and 7am
        if (hour is >= 4 and <= 7)
        {
            price += 12;
        }

        // Add VAT
        price *= 1.05;
        
        // Take into account price cap
        return price >= 33.33 ? 33.33 : Math.Round(price, 2);
    }
    

    public async Task<Dictionary<int, double>?> GetDailyPrices(DateTimeOffset date)
    {
        Dictionary<int, double> prices = new();
        
        string periodFrom = date.ToString("yyyy-MM-ddT00:00");
        string periodTo = date.ToString("yyyy-MM-ddT23:59");
        
        OctopusResponse? octopusResponse = await GetOctopusResponse(periodFrom, periodTo);
        
        if (octopusResponse == null)
        {
            return null;
        }

        int[] hours = Enumerable.Range(0, 24).ToArray();
        
        foreach (Results result in octopusResponse.Results)
        {
            if (result.ValidFrom == null)
            {
                continue;
            }
            
            DateTimeOffset validFrom = DateTimeOffset.Parse(result.ValidFrom);

            if (hours.Contains(validFrom.Hour) == false || prices.ContainsKey(validFrom.Hour))
            {
                continue;
            }
            
            prices.Add(validFrom.Hour, GetFinalPrice(result.ValueExcludingVat, validFrom.Hour));
        }

        return prices;
    }
}