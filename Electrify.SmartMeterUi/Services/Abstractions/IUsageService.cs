using Electrify.Models.Models;

namespace Electrify.SmartMeterUi.Services.Abstractions;

internal interface IUsageService
{
    UsageInstance GetCurrentUsage();
}
