using System.Diagnostics.CodeAnalysis;
using Electrify.SmartMeterUi.Services.Abstraction;

namespace Electrify.SmartMeterUi.Services;

[ExcludeFromCodeCoverage]
public sealed class CommandLineArgsProvider : ICommandLineArgsProvider
{
    public string GetArgAtIndex(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);

        var commandLineArgs = Environment.GetCommandLineArgs();
        
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, commandLineArgs.Length);

        return commandLineArgs[index];
    }
}