using Electrify.SmartMeterUi.Services.Abstractions;
using Microsoft.AspNetCore.Components;
using Blazored.Toast.Services;
using Blazored.Toast.Configuration;

namespace Electrify.SmartMeterUi.Components.Pages;

public partial class SmartMeterHome
{
    private float _currentUsage = 3.45f;
    private float _pricePerKw;
    private float _usagePercent;
    private string _barColour = "#00b5f1";
    private string _dialBackground = string.Empty;
    private readonly int _daysSincePeriodStart = 14;
    private Timer? _timer;
    private float _cumulativeUsage;
    private const string _disconnectMessage = "Smart Meter disconnected. Attepting to reconnect.";

    [Inject] private IUsageService UsageService { get; set; } = default!;

    [Inject] private IToastService ToastService { get; set; } = default!;

    [Inject] private IErrorMessageService ErrorMessageService { get; set; } = default!;

    protected override void OnInitialized()
    {
        GetPrice();
        SetUpTimer();
        UpdateDial();
    }

    private void SetUpTimer()
    {
        _timer = new Timer(OnTimerElapsed, null, 0, 2000);
    }

    private void GetPrice()
    {
        _pricePerKw = 0.2122f;
    }

    private void GetCurrentUsage()
    {
        _currentUsage = UsageService.GetCurrentUsage();
        _usagePercent = (float)Math.Round(_currentUsage * 10000, 6);
        _cumulativeUsage = UsageService.GetCumulativeUsage().Usage;
    }

    private void OnTimerElapsed(object? state)
    {
        GetCurrentUsage();
        InvokeAsync(UpdateDial);
        CheckForNewToastMessage();
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

    private void CheckForNewToastMessage()
    {
        bool connected = ErrorMessageService.IsConnected;
        string? errorMessage = ErrorMessageService.ErrorMessage;

        static void toastOptions(ToastSettings options)
        {
            options.DisableTimeout = true;
            options.ShowCloseButton = false;
        }

        if (!connected)
        {
            ToastService.ShowError(_disconnectMessage, toastOptions);
        }
        else
        {
            ToastService.ClearErrorToasts();
        }

        if (errorMessage != null)
        {
            ToastService.ShowWarning(errorMessage, toastOptions);
        }
        else
        {
            ToastService.ClearWarningToasts();
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    #region UI Display Methods
    private string GetUsageDaily()
    {
        return Math.Round(_cumulativeUsage, 2).ToString();
    }

    private string GetUsagePriceDaily()
    {
        return Math.Round(_pricePerKw * _cumulativeUsage, 2).ToString("C");
    }

    private string GetUsagePeriod()
    {
        return Math.Round(_cumulativeUsage * _daysSincePeriodStart, 2).ToString();
    }

    private string GetUsagePricePeriod()
    {
        return Math.Round(_pricePerKw * _cumulativeUsage * _daysSincePeriodStart, 2).ToString("C");
    }
    #endregion
}