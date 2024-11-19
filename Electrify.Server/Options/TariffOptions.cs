namespace Electrify.Server.Options;

public sealed record TariffOptions
{
    public TimeSpan TariffUpdateInterval { get; set; }
}