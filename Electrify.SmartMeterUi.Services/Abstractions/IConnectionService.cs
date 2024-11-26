namespace Electrify.SmartMeterUi.Services.Abstractions;

public interface IConnectionService
{
    public Task InitializeConnectionAsync(bool isReconnect);
    public Task AttemptReconnectAsync();
    bool InitialConnectionMade {get;}
    void SetClientId(Guid clientId);
}