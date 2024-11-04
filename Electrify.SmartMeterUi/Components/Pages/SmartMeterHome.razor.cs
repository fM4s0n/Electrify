using Electrify.SmartMeterUi.Services.Abstractions;
using Microsoft.AspNetCore.Components;

namespace Electrify.SmartMeterUi.Components.Pages;

public partial class SmartMeterHome
{
    private float _currentUsage = 3.45f;
    private float _pricePerKw = 0f;
    private float _usagePercent = 0;
    private string _barColour = "#00b5f1";
    private string _dialBackground = "";
    private readonly int _hourOfDay = 6;
    private readonly int _daysSincePeriodStart = 14;
    private readonly System.Timers.Timer _timer = new(2000);

    [Inject] private IUsageService? UsageService { get; set; }

    protected override void OnInitialized()
    {
        UsageService!.Start();
        GetPrice();
        SetUpTimer();
        UpdateDial();
    }

    private void SetUpTimer()
    {
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = true; // Make sure the timer resets automatically
        _timer.Start();
    }

    private void GetPrice()
    {
        _pricePerKw = 0.2122f;
    }

    private void GetCurrentUsage()
    {
        _currentUsage = UsageService!.GetCurrentUsage().Usage;
        _usagePercent = (float)Math.Round(_currentUsage * 10000, 6);
    }

    private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        GetCurrentUsage();
        InvokeAsync(UpdateDial);
    }

    private void UpdateDial()
    {
        double adjustedPercentage = (_usagePercent / 100.0) * 270 + 45;

        if (_usagePercent <= 70)
        {
            _barColour = "#00b5f1"; // Blue
        }
        else if (_usagePercent <= 90)
        {
            _barColour = "#FFA500"; // Orange
        }
        else
        {
            _barColour = "#FF0000"; // Red
        }

        _dialBackground = $"conic-gradient({_barColour} 45deg, {_barColour} {adjustedPercentage}deg, #72777E {adjustedPercentage}deg, #72777E 360deg)";
        StateHasChanged();
    }

    #region UI Display Methods
    private string GetUsageDaily()
    {
        return Math.Round((_currentUsage * _hourOfDay), 2).ToString();
    }

    private string GetUsagePriceDaily()
    {
        return Math.Round((_pricePerKw * _currentUsage * _hourOfDay), 2).ToString("C");
    }

    private string GetUsagePeriod()
    {
        return Math.Round((_currentUsage * _hourOfDay * _daysSincePeriodStart), 2).ToString();
    }

    private string GetUsagePricePeriod()
    {
        return Math.Round((_pricePerKw * _currentUsage * _hourOfDay * _daysSincePeriodStart), 2).ToString("C");
    }
    #endregion
}