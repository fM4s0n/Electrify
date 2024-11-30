
namespace Electrify.SmartMeterUi.Services.Abstraction;

public interface IErrorMessageService
{
    bool IsConnected { get; set; }

    string? ErrorMessage { get; set; }
}
