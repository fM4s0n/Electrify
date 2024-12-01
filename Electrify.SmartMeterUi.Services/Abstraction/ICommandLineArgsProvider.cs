namespace Electrify.SmartMeterUi.Services.Abstraction;

public interface ICommandLineArgsProvider
{
    public string GetArgAtIndex(int index);
}