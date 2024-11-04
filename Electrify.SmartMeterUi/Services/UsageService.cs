using Electrify.Models.Models;
using Electrify.SmartMeterUi.Services.Abstractions;

namespace Electrify.SmartMeterUi.Services;

internal class UsageService : IUsageService
{
    public List<UsageInstance> UsageHistory = [];
    private System.Timers.Timer? _randomTimer;
    private readonly static Random _random = new();

    public void Start()
    {
        UsageHistory.Clear();

        AddNewReading();

        SetUpTimer();
    }

    public void Stop()
    {
        _randomTimer?.Stop();
        _randomTimer?.Dispose();
    }

    private void SetUpTimer()
    {
        _randomTimer = new(GetRandomTimerInterval());
        _randomTimer.Elapsed += OnTimerElapsed;
        _randomTimer.AutoReset = false;
        _randomTimer.Start();
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

    internal static float GenerateRandomUsage()
    {
        //Random random = new();
        //float randomUsage = (float)(random.NextDouble() * 5);
        //return randomUsage;

        Random random = new();
        float randomFloat = (float)(random.NextDouble() * (0.00999 - 0.00100) + 0.00100);
        return randomFloat;
    }

    private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        AddNewReading();

        _randomTimer!.Interval = GetRandomTimerInterval();
        _randomTimer.Start();
    }

    public UsageInstance GetCurrentUsage()
    {
        return UsageHistory.Last();
    }

    /// <summary>
    /// Gets a new random timer ineterval between 15 and 60 seconds
    /// </summary>
    /// <returns></returns>
    private double GetRandomTimerInterval()
    {
        //return _random.Next(15000, 60001);
        return 2000;
    }
}
