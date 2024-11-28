using Electrify.AdminUi.Services.Abstractions;
using Electrify.Models;
using Electrify.Server.ApiClient.Abstraction;

namespace Electrify.AdminUi.Services;

public class ClientService(IElectrifyApiClient electrifyApiClient, IAdminService adminService) : IClientService
{
    public async Task<bool> InsertClient(Client newClient)
    {
        if (adminService.CurrentAdmin is null)
        {
            return false;
        }
        
        var response = await electrifyApiClient.InsertClient(adminService.CurrentAdmin.Token, newClient.Id, newClient.UserId);
        return response.Success;
    }
}
