namespace Electrify.Models.Models;

public sealed record Client
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
}