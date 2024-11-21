using Electrify.Models.Enums;

namespace Electrify.Models;

public sealed record ToastDefinition
{
    public required string Title { get; init; }
    public required string Message { get; init; }
    public required ToastType Type { get; init; }
}
