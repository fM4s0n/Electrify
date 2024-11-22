using Electrify.Protos;
using Electrify.Server.Services.Abstraction;
using Grpc.Core;

namespace Electrify.Server.Services;

public class AuthenticationService(IClientService clientService) : Authentication.AuthenticationBase
{
    public override async Task<AuthenticateResponse> Authenticate(AuthenticateRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.UserId, out var userId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "UserId must be a valid Guid"));
        }
        
        if (!Guid.TryParse(request.ClientId, out var clientId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "ClientId must be a valid Guid"));
        }

        var clientExists = await clientService.ClientExists(userId, clientId);
        
        return new AuthenticateResponse
        {
            Success = clientExists,
        };
    }
}