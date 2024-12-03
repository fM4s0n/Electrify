using Electrify.Models;
using Electrify.Protos;
using Electrify.Server.Services.Abstraction;
using Grpc.Core;

namespace Electrify.Server.Services;

public class InsertClientService(
    IClientService clientService,
    ILogger<InsertClientService> logger,
    IAdminService adminService)
    : InsertClient.InsertClientBase
{
    public override async Task<InsertClientResponse> InsertClient(InsertClientRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.UserId, out var userId))
        {
            logger.LogError("UserId was not a valid Guid : {UserId}", request.UserId);
            throw new RpcException(new Status(StatusCode.InvalidArgument, "UserId must be a valid Guid"));
        }

        if (!Guid.TryParse(request.Id, out var clientId))
        {
            logger.LogError("ClientId was not a valid Guid : {ClientId}", request.Id);
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Client Id must be a valid Guid"));
        }

        var newClient = new Client
        {
            Id = clientId,
            UserId = userId,
        };

        bool success = await clientService.InsertClient(newClient);

        return new InsertClientResponse
        {
            Success = success,
        };
    }
}
