namespace Electrify.SmartMeterUi.Components.Pages;

public partial class SmartMeterHome
{
    private float _currentUsage = 3.45f;
    private float _pricePerKw = 0.2122f;
    private float _usagePercent = 0;
    private string _barColour = "#00b5f1";
    private string _dialBackground = "";
    private int _hourOfDay = 6;
    private int _daysSincePeriodStart = 14;
    private System.Timers.Timer _timer;

    protected override void OnInitialized()
    {
        SetUpTimer();
        UpdateDial();
    }

    private void SetUpTimer()
    {
        _timer = new System.Timers.Timer(2000);
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = true; // Make sure the timer resets automatically
        _timer.Start();
    }

    private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        InvokeAsync(UpdateDial);
    }
    
    private void UpdateDial()
    {
        RandomiseUsage();
        
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

    private void RandomiseUsage()
    {
        Random random = new();
        _currentUsage = (float)(random.NextDouble() * 5);
        _usagePercent = (float)Math.Round(_currentUsage * 20, 2);
    }

    private string GetUsageDaily()
    {
        return Math.Round((_currentUsage * _hourOfDay), 2).ToString();
    }
    
    private string GetUsagePriceDaily()
    {
        return Math.Round((_pricePerKw * _currentUsage * _hourOfDay), 2).ToString();
    }
    
    private string GetUsagePeriod()
    {

        return Math.Round((_currentUsage * _hourOfDay * _daysSincePeriodStart), 2).ToString();
    }
    
    private string GetUsagePricePeriod()
    {
        return Math.Round((_pricePerKw * _currentUsage * _hourOfDay * _daysSincePeriodStart), 2).ToString();
    }
}