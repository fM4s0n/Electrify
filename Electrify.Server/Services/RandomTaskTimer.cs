namespace Electrify.Server.Services;

/// <summary>
/// After executing the callback the timer changes its period to a value between
/// the StartRange and EndRange it was initialised with.
/// </summary>
public class RandomTaskTimer : IDisposable, IAsyncDisposable
{
    private readonly ITimer _timer;
    private readonly Random _random = new();
    private readonly int _startRange;
    private readonly int _endRange;

    public RandomTaskTimer(TimeProvider timeProvider, Action callback, int startRange, int endRange)
    {
        _startRange = startRange;
        _endRange = endRange;

        _timer = timeProvider.CreateTimer(
            _ => Callback(callback),
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(_random.Next(_startRange, _endRange)));
    }

    private void Callback(Action callback)
    {
        callback.Invoke();
        _timer.Change(TimeSpan.FromSeconds(_random.Next(_startRange, _endRange)),
            TimeSpan.FromSeconds(_random.Next(_startRange, _endRange)));
    }
    
    public void Dispose()
    {
        _timer.Dispose();
    }
    
    public async ValueTask DisposeAsync()
    {
        await _timer.DisposeAsync();
    }
}