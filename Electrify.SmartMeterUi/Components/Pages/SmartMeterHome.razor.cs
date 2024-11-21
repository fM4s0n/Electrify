using Electrify.Dlms.Server.Abstraction;
using Electrify.SmartMeterUi.Services.Abstractions;
using Microsoft.AspNetCore.Components;
using Electrify.SmartMeterUi.Enums;

namespace Electrify.SmartMeterUi.Components.Pages;

public partial class SmartMeterHome
{
    private float _currentUsage = 3.45f;
    private float _pricePerKw;
    private float _usagePercent;
    private string _barColour = "#00b5f1";
    private string _dialBackground = string.Empty;
    private Timer? _timer;
    private const string DisconnectMessage = "Smart Meter disconnected. Attempting to reconnect.";
    private readonly List<Toast> _displayToasts = [];

    [Inject] private IUsageService UsageService { get; set; } = default!;
    [Inject] private IErrorMessageService ErrorMessageService { get; set; } = default!;
    [Inject] private IDlmsServer DlmsServer { get; set; } = default!;

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
        //_cumulativeUsage = UsageService.GetCumulativeUsage().Usage;
    }

    private void OnTimerElapsed(object? state)
    {
        GetCurrentUsage();
        InvokeAsync(UpdateDial);
        CheckForNewToastMessage();
        GetReadings();
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

        if (!connected)
        {
            if (_displayToasts.FirstOrDefault(t => t.Type == ToastEnum.NoConnection) == null)
            {
                _displayToasts.Add(new Toast
                {
                    Title = "No Connection",
                    Message = DisconnectMessage,
                    Type = ToastEnum.NoConnection
                });
            }
        }
        else
        {
            _displayToasts.Remove(_displayToasts.FirstOrDefault(t => t.Type == ToastEnum.NoConnection)!);
        }

        if (errorMessage != null)
        {
            if (_displayToasts.FirstOrDefault(t => t.Type == ToastEnum.ServerUpdate) == null)
            {
                _displayToasts.Add(new Toast
                {
                    Title = "Server Update",
                    Message = errorMessage,
                    Type = ToastEnum.ServerUpdate
                });
            }
        }
        else
        {
            _displayToasts.Remove(_displayToasts.FirstOrDefault(t => t.Type == ToastEnum.ServerUpdate)!);
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    #region UI Display Methods
    private string GetUsageDaily()
    {
        return "";
        //return Math.Round(_cumulativeUsage, 2).ToString();
    }

    private string GetUsagePriceDaily()
    {
        return "";
        //return Math.Round(_pricePerKw * _cumulativeUsage, 2).ToString("C");
    }

    private string GetUsagePeriod()
    {
        return "";
        //return Math.Round(_cumulativeUsage * _daysSincePeriodStart, 2).ToString();
    }

    private string GetUsagePricePeriod()
    {
        return "";
        //return Math.Round(_pricePerKw * _cumulativeUsage * _daysSincePeriodStart, 2).ToString("C");
    }
    #endregion

    private class Toast()
    {
        public required string Title { get; init; }
        public required string Message { get; init; }
        public required ToastEnum Type { get; init; }
    }

    private void GetReadings()
    {
        // Create datetime object
        DateTime now = DateTime.Now;
        
        // Calculate DateTime for day period until now
        DateTime dayStart = new(now.Year, now.Month, now.Day);
        var dailyTotal = CalculatePeriodReading(dayStart);
        
        // Calculate DateTime for month period until now
        DateTime monthStart = new (now.Year, now.Month, 1);
        var monthlyTotal = CalculatePeriodReading(monthStart);
    }

    private Tuple<double, double> CalculatePeriodReading(DateTime periodStart)
    {
        // Grab the total readings
        var readings = DlmsServer.GetReadings().ToList();

        if (readings.Count == 0)
            return new(0.00, 0.00);

        if (!readings.Any(r => r.DateTime > periodStart))
            return new Tuple<double, double>(0.00, 0.00);

        if (readings.Count(r => r.DateTime >= periodStart) == 1)
            return new Tuple<double, double>(readings[0].EnergyUsage * readings[0].Tariff, readings[0].EnergyUsage);

        double totalPrice = 0.0;
        double totalUsage = 0.0;
        
        for (var i = 0; i < readings.Count; i++)
        {
            var reading = readings[i];
            if (reading.DateTime < periodStart)
                continue;
            
            var lastReading = readings[i - 1];

            totalPrice += (reading.EnergyUsage - lastReading.EnergyUsage) * reading.Tariff;
            totalUsage += reading.EnergyUsage - lastReading.EnergyUsage;
        }

        return new(totalPrice, totalUsage);
    }
    
}