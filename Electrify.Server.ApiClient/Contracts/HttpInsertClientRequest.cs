namespace Electrify.Server.ApiClient.Contracts;

public sealed record HttpInsertClientRequest
{
    public required string Token { get; set; }
    public required string Id { get; set; }
    public required string UserId { get; set; }
}
