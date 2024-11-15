
namespace Electrify.SmartMeterUi.Services.Abstractions;

public interface IErrorMessageService
{
    bool IsConnected { get; set; }

    string? ErrorMessage { get; set; }
}
