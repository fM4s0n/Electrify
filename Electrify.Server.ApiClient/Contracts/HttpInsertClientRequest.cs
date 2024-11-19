namespace Electrify.Server.ApiClient.Contracts;

public sealed record HttpInsertClientRequest
{
    public required string Id { get; set; } = string.Empty;
    public required string UserId { get; set; } = string.Empty;
}
