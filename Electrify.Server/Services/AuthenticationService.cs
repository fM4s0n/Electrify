using Electrify.DlmsServer;
using Electrify.Server.Services.Abstraction;
using Grpc.Core;

namespace Electrify.Server.Services;

public class AuthenticationService(IClientService clientService, ILogger<AuthenticationService> logger) : Authentication.AuthenticationBase
{
    public override async Task<AuthenticateResponse> Authenticate(AuthenticateRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.UserId, out var userId))
        {
            logger.LogError("UserId was not a valid Guid : {UserId}", request.UserId);
            throw new RpcException(new Status(StatusCode.InvalidArgument, "UserId must be a valid Guid"));
        }
        
        if (!Guid.TryParse(request.ClientId, out var clientId))
        {
            logger.LogError("ClientId was not a valid Guid : {ClientId}", request.ClientId);
            throw new RpcException(new Status(StatusCode.InvalidArgument, "ClientId must be a valid Guid"));
        }

        var clientExists = await clientService.ClientExists(userId, clientId);
        
        return new AuthenticateResponse
        {
            Success = clientExists,
        };
    }
}