namespace Electrify.Server.ApiClient.Contracts;

public sealed record HttpAdminLoginResponse
{
    public required bool Success { get; init; }
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init;}
    public required string Token { get; init; }
}
