namespace Electrify.Models.Models;

public sealed record Client
{
    public required Guid UserId { get; set; }
    public required Guid ClientId { get; set; }
}