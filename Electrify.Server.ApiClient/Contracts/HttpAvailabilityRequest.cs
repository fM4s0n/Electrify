namespace Electrify.Server.ApiClient.Contracts;

public sealed record HttpAvailabilityRequest
{
    public int Port { get; set; }
    public string Secret { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
}
