using Electrify.Protos;
using Electrify.Server.Services.Abstraction;
using Grpc.Core;

namespace Electrify.Server.Services;

public class ConnectedClientsService(
    IAdminService adminService,
    IDlmsClientService dlmsClientService)
    : ConnectedClients.ConnectedClientsBase
{
    public override async Task<GetConnectedClientIdsResponse> GetConnectedClientIds(
        GetConnectedClientIdsRequest request,
        ServerCallContext context)
    {
        var response = new GetConnectedClientIdsResponse();
        
        foreach (var dlmsClient in dlmsClientService.GetClients())
        {
            response.ClientIds.Add(dlmsClient.ClientId.ToString());
        }

        return response;
    }
}