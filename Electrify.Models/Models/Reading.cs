using Microsoft.EntityFrameworkCore;

namespace Electrify.Models.Models;

[PrimaryKey(nameof(DateTime), nameof(ClientId))]
public sealed record Reading
{
    public required Guid ClientId { get; init; }
    public required DateTime DateTime { get; init; }
    public required double EnergyUsage { get; init; }
    public required double Tariff { get; init; }
}
