using Electrify.Models.Models;

namespace Electrify.Server.Services.Abstraction;

public interface IClientService
{
    Task<bool> ClientExists(Guid userId, Guid clientId);

    Task<bool> InsertClient(Client client);
}