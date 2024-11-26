namespace Electrify.SmartMeterUi.Services.Abstractions;

public interface IClientRegistrationService
{
    Task<bool> ClientIdExists(Guid clientId);
}
