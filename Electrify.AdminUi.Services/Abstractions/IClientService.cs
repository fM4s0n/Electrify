using Electrify.Models.Models;

namespace Electrify.AdminUi.Services.Abstractions;

public interface IClientService
{
    Task<bool> InsertClient(Client newClient);
}
