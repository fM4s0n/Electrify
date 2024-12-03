using System.Diagnostics.CodeAnalysis;
using Electrify.SmartMeterUi.Services.Abstraction;

namespace Electrify.SmartMeterUi.Services;

[ExcludeFromCodeCoverage]
public sealed class CommandLineArgsProvider : ICommandLineArgsProvider
{
    private readonly string[] _commandLineArgs = Environment.GetCommandLineArgs();
    
    public string GetArgAtIndex(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _commandLineArgs.Length);

        return _commandLineArgs[index];
    }
}