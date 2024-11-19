using Electrify.AdminUi.Services.Abstractions;
using Electrify.Models.Models;
using Electrify.Server.ApiClient.Abstraction;

namespace Electrify.AdminUi.Services;

public class ClientService(IElectrifyApiClient electrifyApiClient) : IClientService
{
    public async Task<bool> InsertClient(Client newClient)
    {
        var response = await electrifyApiClient.InsertClient(newClient.Id, newClient.UserId);
        return response.Success;
    }
}
