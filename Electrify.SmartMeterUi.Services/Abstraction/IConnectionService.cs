namespace Electrify.SmartMeterUi.Services.Abstraction;

public interface IConnectionService
{
    public Task InitializeConnectionAsync(bool isReconnect);
    public Task AttemptReconnectAsync();
    bool InitialConnectionMade {get;}
}