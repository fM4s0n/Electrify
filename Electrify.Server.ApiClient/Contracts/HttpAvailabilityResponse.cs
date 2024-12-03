namespace Electrify.Server.ApiClient.Contracts;

public sealed record HttpAvailabilityResponse
{
    public bool Success { get; set; }
    public IEnumerable<HttpHistoricReading> HistoricReadings { get; set; }
}

public sealed record HttpHistoricReading
{
    public string Timestamp { get; set; }
    public double Usage { get; set; }
    public double Tariff { get; set; }
}