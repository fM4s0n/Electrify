using Electrify.Models;

namespace Electrify.SmartMeterUi.Services.Abstraction;

public interface IUsageService
{
    UsageInstance GetCumulativeUsage();

    float GetCurrentUsage();
}
