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
        if (!Guid.TryParse(request.Token, out var token))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Token must be in GUID format"));
        }

        if (!await adminService.ValidateToken(token))
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Token is invalid"));
        }

        var response = new GetConnectedClientIdsResponse();
        
        foreach (var dlmsClient in dlmsClientService.GetClients())
        {
            response.ClientIds.Add(dlmsClient.ClientId.ToString());
        }

        return response;
    }
}