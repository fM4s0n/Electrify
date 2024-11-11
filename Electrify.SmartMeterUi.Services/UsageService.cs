﻿using Electrify.Models.Models;
using Electrify.SmartMeterUi.Services.Abstractions;

namespace Electrify.SmartMeterUi.Services;

public class UsageService : IUsageService, IDisposable
{
    private readonly List<UsageInstance> _usageHistory = [];
    private readonly Timer? _timer;
    private readonly Random _random = new();

    public UsageService()
    {
        _usageHistory.Clear();
        AddNewReading();
        _timer = new Timer(OnTimerElapsed, null, 500, Timeout.Infinite);
    }

    private void AddNewReading()
    {
        UsageInstance newReading = new()
        {
            TimeStamp = DateTime.Now,
            Usage = GenerateRandomUsage()
        };

        _usageHistory.Add(newReading);
    }

    private float GenerateRandomUsage()
    {
        float randomFloat = (float)(_random.NextDouble() * (0.00999 - 0.00100) + 0.00100);
        return randomFloat;
    }

    private void OnTimerElapsed(object? sender)
    {
        AddNewReading();
        _timer?.Change(1000, Timeout.Infinite);
    }

    public UsageInstance GetCurrentUsage()
    {
        return _usageHistory.Last();
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}