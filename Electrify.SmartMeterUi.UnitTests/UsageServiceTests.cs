using Electrify.SmartMeterUi.Services;

namespace Electrify.SmartMeterUi.UnitTests;

public class UsageServiceTests
{
    private readonly UsageService _usageService;

    public UsageServiceTests()
    {
        _usageService = new UsageService();
    }
}