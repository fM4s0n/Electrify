namespace Electrify.Models.Models;

/// <summary>
/// 
/// </summary>
public sealed record class UsageInstance
{
    /// <summary>
    /// 
    /// </summary>
    public required DateTime TimeStamp { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public required float Usage { get; init; }
}
