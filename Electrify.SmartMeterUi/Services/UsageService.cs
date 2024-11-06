using Electrify.Models.Models;
using Electrify.SmartMeterUi.Services.Abstractions;

namespace Electrify.SmartMeterUi.Services;

internal partial class UsageService : IUsageService, IDisposable
{
    public List<UsageInstance> UsageHistory = [];
    private readonly Timer? _randomTimer;
    private readonly static Random _random = new();

    public UsageService()
    {
        UsageHistory.Clear();
        AddNewReading();
        _randomTimer = new(OnTimerElapsed, null, GetRandomTimerInterval(), Timeout.Infinite);
    }

    internal void AddNewReading()
    {
        UsageInstance newReading = new()
        {
            TimeStamp = DateTime.Now,
            Usage = GenerateRandomUsage()
        };

        UsageHistory.Add(newReading);
    }

    private static float GenerateRandomUsage()
    {
        float randomFloat = (float)(_random.NextDouble() * (0.00999 - 0.00100) + 0.00100);
        return randomFloat;
    }

    private void OnTimerElapsed(object? sender)
    {
        AddNewReading();
        _randomTimer?.Change(GetRandomTimerInterval(), Timeout.Infinite);
    }

    public UsageInstance GetCurrentUsage()
    {
        return UsageHistory.Last();
    }

    /// <summary>
    /// Gets a new random timer ineterval between 15 and 60 seconds
    /// </summary>
    /// <returns></returns>
    private static int GetRandomTimerInterval()
    {
        return _random.Next(15000, 60001);
    }

    public void Dispose()
    {
        _randomTimer?.Dispose();
    }
}
