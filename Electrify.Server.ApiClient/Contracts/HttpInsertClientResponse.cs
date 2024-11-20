namespace Electrify.Server.ApiClient.Contracts;

public sealed record HttpInsertClientResponse
{
    public required bool Success { get; set; }
}
