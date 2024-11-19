namespace Electrify.Dlms.Models;

public sealed record GenericProfileRow
{
    public required DateTime DateTime { get; init; }
    public required double EnergyUsage { get; init; }
    public required double Tariff { get; init; }
}