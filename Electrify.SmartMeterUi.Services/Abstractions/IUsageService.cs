using Electrify.Models;

namespace Electrify.SmartMeterUi.Services.Abstractions;

public interface IUsageService
{
    UsageInstance GetCumulativeUsage();

    float GetCurrentUsage();
}
