using Electrify.Dlms.Server.Abstraction;
using Electrify.SmartMeterUi.Services;
using NSubstitute;

namespace Electrify.SmartMeterUi.UnitTests;

public class UsageServiceTests
{
    private readonly UsageService _usageService;

    public UsageServiceTests()
    {
        _usageService = new UsageService(Substitute.For<IDlmsServer>());
    }
}