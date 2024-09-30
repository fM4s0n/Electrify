using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Electrify.Dlms.Extensions;

public static class LoggerExtensions
{
    public static LogLevel CurrentLogLevel(this ILogger logger)
    {
        foreach(LogLevel logLevel in Enum.GetValues(typeof(LogLevel)))
        {
            if( logger.IsEnabled(logLevel))
                return logLevel;
        }

        return LogLevel.None;
    }

    public static TraceLevel ToTraceLevel(this LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => TraceLevel.Verbose,
            LogLevel.Debug => TraceLevel.Verbose,
            LogLevel.Information => TraceLevel.Info,
            LogLevel.Warning => TraceLevel.Warning,
            LogLevel.Error => TraceLevel.Error,
            LogLevel.Critical => TraceLevel.Error,
            LogLevel.None => TraceLevel.Off,
            _ => throw new UnreachableException(),
        };
    }
}