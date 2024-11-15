namespace Electrify.Models;

public sealed record UsageInstance
{
    /// <summary>
    /// The time the meter was read
    /// </summary>
    public required DateTime TimeStamp { get; init; }

    /// <summary>
    /// The value from the meter in kw/h
    /// </summary>
    public required float Usage { get; init; }
}
