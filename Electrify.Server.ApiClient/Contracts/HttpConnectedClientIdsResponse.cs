namespace Electrify.Server.ApiClient.Contracts;

public sealed record HttpConnectedClientIdsResponse
{
    public IEnumerable<string> ClientIds { get; set; } = [];
}