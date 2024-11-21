namespace Electrify.Server.ApiClient.Contracts;

public sealed record HttpAdminLoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
