using System.Diagnostics;
using Gurux.DLMS.Enums;

namespace Electrify.Dlms.Options;

public sealed class DlmsServerOptions
{
    public int Port { get; init; }
    public string Password { get; init; } = string.Empty;
    public Authentication Authentication { get; init; }
    public TraceLevel TraceLevel { get; init; }
    public uint GenericProfileCapturePeriod { get; init; }
}