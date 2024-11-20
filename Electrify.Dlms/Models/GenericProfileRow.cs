using CsvHelper.Configuration.Attributes;

namespace Electrify.Dlms.Models;

public class GenericProfileRow
{
    [Index(0)]
    public required DateTime DateTime { get; set; }
    [Index(1)]
    public required double EnergyUsage { get; set; }
    [Index(2)]
    public required double Tariff { get; set; }
}