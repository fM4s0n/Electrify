using Electrify.Models;

namespace Electrify.SmartMeterUi.Services.Abstraction;

public interface IUsageService
{
    void SetLastUsage(DateTime dateTime, double usage);
    
    UsageInstance GetCumulativeUsage();

    float GetCurrentUsage();
}
