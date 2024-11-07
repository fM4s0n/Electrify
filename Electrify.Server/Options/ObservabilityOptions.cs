using System.Diagnostics;

namespace Electrify.Server.Options;

public sealed record ObservabilityOptions
{
    public LogLevel LogLevel { get; set; }
    public TraceLevel TraceLevel { get; set; }
}