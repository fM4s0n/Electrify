using Electrify.Models;

namespace Electrify.AdminUi.Services.Abstractions;

public interface IClientService
{
    Task<bool> InsertClient(Client newClient);
}
