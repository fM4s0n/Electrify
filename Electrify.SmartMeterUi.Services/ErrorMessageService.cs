using Electrify.SmartMeterUi.Services.Abstractions;

namespace Electrify.SmartMeterUi.Services;

public class ErrorMessageService : IErrorMessageService
{
    public bool IsConnected { get; set; }

    public string? ErrorMessage { get; set; }
}
