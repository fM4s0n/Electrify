using Electrify.Dlms.Models;
using Electrify.Dlms.Server.Abstraction;
using Electrify.SmartMeterUi.Services.Abstractions;
using Microsoft.AspNetCore.Components;
using Electrify.Models;
using Electrify.Models.Enums;

namespace Electrify.SmartMeterUi.Components.Pages;

public partial class SmartMeterHome
{
    private float _currentUsage = 3.45f;
    private float _usagePercent;
    private string _barColour = "#00b5f1";
    private string _dialBackground = string.Empty;
    private Timer? _timer;
    private const string DisconnectMessage = "Smart Meter disconnected. Attempting to reconnect.";
    private readonly List<ToastDefinition> _displayToasts = [];
    private Tuple<double, double> _dailyReadingCost = new(0.00, 0.00);
    private Tuple<double, double> _monthlyReadingCost = new(0.00, 0.00);

    [Inject] private IUsageService UsageService { get; set; } = default!;
    [Inject] private IErrorMessageService ErrorMessageService { get; set; } = default!;
    [Inject] private IDlmsServer DlmsServer { get; set; } = default!;
    [Inject] private IConnectionService ConnectionService { get; set; } = default!;

    protected override void OnInitialized()
    {
        SetUpTimer();
        UpdateDial();
    }

    private void SetUpTimer()
    {
        _timer = new Timer(OnTimerElapsed, null, 0, 2000);
    }

    private void GetCurrentUsage()
    {
        _currentUsage = UsageService.GetCurrentUsage();
        _usagePercent = (float)Math.Round(_currentUsage * 10000, 6);
    }

    private async void OnTimerElapsed(object? state)
    {
        GetCurrentUsage();
        UpdateDial();
        await CheckForNewToastMessage();
        GetReadings();
        await InvokeAsync(StateHasChanged);
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
    }

    private async Task CheckForNewToastMessage()
    {
        bool connected = ErrorMessageService.IsConnected;
        string? errorMessage = ErrorMessageService.ErrorMessage;

        if (!connected)
        {
            if (_displayToasts.FirstOrDefault(t => t.Type == ToastType.NoConnection) == null)
            {
                _displayToasts.Add(new ToastDefinition
                {
                    Title = "No Connection",
                    Message = DisconnectMessage,
                    Type = ToastType.NoConnection
                });
            }

            if (ConnectionService.InitialConnectionMade)
            {
                try
                {
                    await ConnectionService.AttemptReconnectAsync();
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Reconnection attempt was canceled.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error during reconnection: {ex.Message}");
                }
            }
        }
        else
        {
            _displayToasts.Remove(_displayToasts.FirstOrDefault(t => t.Type == ToastType.NoConnection)!);
        }

        if (errorMessage != null)
        {
            if (_displayToasts.FirstOrDefault(t => t.Type == ToastType.ServerUpdate) == null)
            {
                _displayToasts.Add(new ToastDefinition
                {
                    Title = "Server Update",
                    Message = errorMessage,
                    Type = ToastType.ServerUpdate
                });
            }
        }
        else
        {
            _displayToasts.Remove(_displayToasts.FirstOrDefault(t => t.Type == ToastType.ServerUpdate)!);
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    private void GetReadings()
    {
        // Create datetime object
        DateTime now = DateTime.Now;
        
        // Calculate DateTime for day period until now
        DateTime dayStart = new(now.Year, now.Month, now.Day);
        _dailyReadingCost = CalculatePeriodReading(dayStart);
        
        // Calculate DateTime for month period until now
        DateTime monthStart = new (now.Year, now.Month, 1);
        _monthlyReadingCost = CalculatePeriodReading(monthStart);
    }

    private Tuple<double, double> CalculatePeriodReading(DateTime periodStart)
    {
        // Get the readings from the server and sort them by datetime
        var readings = DlmsServer.GetReadings()
            .OrderBy(r => r.DateTime)
            .ToList();

        if (readings.Count == 0)
            return new Tuple<double, double>(0.00, 0.00);

        // Find the last reading before the cutoff
        var lastBeforePeriod = readings.LastOrDefault(r => r.DateTime < periodStart);
        var startingEnergy = lastBeforePeriod?.EnergyUsage ?? 0.0;

        // Filter readings that are within the period
        var periodReadings = readings.Where(r => r.DateTime >= periodStart).ToList();

        if (periodReadings.Count == 0)
            return new Tuple<double, double>(0.00, 0.00);

        double totalPrice = 0.0;
        double totalUsage = 0.0;

        // Iterate through the readings in the period to calculate the total cost and usage
        for (int i = 0; i < periodReadings.Count; i++)
        {
            GenericProfileRow reading = periodReadings[i];

            double previousEnergy = i == 0 ? startingEnergy : periodReadings[i - 1].EnergyUsage;

            double usageInPeriod = reading.EnergyUsage - previousEnergy;
            totalUsage += usageInPeriod;
            totalPrice += usageInPeriod * reading.Tariff;
        }

        return new Tuple<double, double>(totalPrice, totalUsage);
    }

    private void Close()
    {
        DlmsServer.Close();
        Application.Current!.Quit();
    }

    #region UI Formatting

    private static string FormatCurrency(double currency)
    {
        return currency.ToString("C");
    }

    private static string FormatKwh(double kwh)
    {
        return Math.Round(kwh, 3) + "kw/h";
    }
    
    #endregion
}