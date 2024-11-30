using Electrify.Dlms.Server.Abstraction;
using Electrify.Models;
using Electrify.SmartMeterUi.Services.Abstraction;

namespace Electrify.SmartMeterUi.Services;

public class UsageService : IUsageService, IDisposable
{
    private UsageInstance _lastUsage;
    private float _currentKwh;
    private readonly Timer? _timer;
    private readonly Random _random = new();
    private readonly IDlmsServer _dlmsServer;
    
    public UsageService(IDlmsServer dlmsServer)
    {
        _dlmsServer = dlmsServer;
        _lastUsage = new UsageInstance
        {
            TimeStamp = DateTime.Now,
            Usage = 0f,
        };
        AddNewReading();
        _timer = new Timer(OnTimerElapsed, null, 1000, Timeout.Infinite);
    }

    /// <summary>
    /// Cumulatively updates the lastUsage property
    /// </summary>
    private void AddNewReading()
    {
        _currentKwh = GenerateRandomUsage();
        UsageInstance newReading = new()
        {
            TimeStamp = DateTime.Now,
            Usage = _lastUsage.Usage + _currentKwh
        };
        _lastUsage = newReading;
        UpdateEnergyUsage(_lastUsage.Usage);
    }

    /// <summary>
    /// Generates a random kw/h usage between 0.001 and 0.00999
    /// </summary>
    /// <returns></returns>
    private float GenerateRandomUsage()
    {
        float randomFloat = (float)(_random.NextDouble() * (0.00999 - 0.00100) + 0.00100);
        return randomFloat;
    }

    /// <summary>
    /// Updates the last usage and re-queues the reading timer
    /// </summary>
    /// <param name="sender"></param>
    private void OnTimerElapsed(object? sender)
    {
        AddNewReading();
        _timer?.Change(1000, Timeout.Infinite);
    }

    /// <summary>
    /// Returns the most recent usage reading
    /// </summary>
    /// <returns></returns>
    public UsageInstance GetCumulativeUsage()
    {
        return _lastUsage;
    }

    public float GetCurrentUsage()
    {
        return _currentKwh;
    }

    private void UpdateEnergyUsage(double usage)
    {
        _dlmsServer.SetEnergy(usage);
    }

    public void Dispose()
    {
        _timer?.Dispose();
        GC.SuppressFinalize(this);
    }
}
