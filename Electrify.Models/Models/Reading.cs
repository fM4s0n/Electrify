namespace Electrify.Models.Models;

public sealed record Reading
{
    public required int Id { get; set; }
    public required int ClientId { get; set; }
    public required string MeterReading { get; set; }
}
