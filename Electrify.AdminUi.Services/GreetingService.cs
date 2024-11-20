using Electrify.AdminUi.Services.Abstractions;

namespace Electrify.AdminUi.Services;

public class GreetingService(TimeProvider timeProvider) : IGreetingService
{
    private readonly TimeProvider _timeProvider = timeProvider;

    public string GetGreeting(string name)
    {
        string timeOfDayGreeting = _timeProvider.GetLocalNow().Hour switch
        {
            < 12 => "Good Morning",
            < 17 => "Good Afternoon",
            _ => "Good Evening"
        };

        return $"{timeOfDayGreeting}, {name}";
    }
}
