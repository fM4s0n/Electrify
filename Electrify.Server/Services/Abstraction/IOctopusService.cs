namespace Electrify.Server.Services.Abstraction;

public interface IOctopusService
{
    Task<double?> GetPrice(DateTimeOffset readingTime);

    Task<Dictionary<int, double>> GetDailyPrices(DateTimeOffset date);
}