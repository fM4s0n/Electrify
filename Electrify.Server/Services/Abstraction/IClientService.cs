namespace Electrify.Server.Services.Abstraction;

public interface IClientService
{
    Task<bool> ClientExists(Guid userId, Guid clientId);
}