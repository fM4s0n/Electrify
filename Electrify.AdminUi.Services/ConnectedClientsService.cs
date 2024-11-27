using Electrify.AdminUi.Services.Abstractions;
using Electrify.Server.ApiClient.Abstraction;

namespace Electrify.AdminUi.Services;

public sealed class ConnectedClientsService(
    IElectrifyApiClient electrifyApiClient,
    AdminService adminService)
    : IConnectedClientsService
{
    public async Task<IEnumerable<string>> GetConnectedClientIds()
    {
        if (adminService.CurrentAdmin is null)
        {
            return [];
        }
        
        return await electrifyApiClient.GetConnectedClientIds(adminService.CurrentAdmin.Token);
    }
}