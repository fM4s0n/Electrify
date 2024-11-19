using Electrify.Models.Models;
using Electrify.Server.Protos;
using Electrify.Server.Services.Abstraction;
using Grpc.Core;

namespace Electrify.Server.Services;

public class InsertClientService(IClientService clientService) : InsertClient.InsertClientBase
{
    public override async Task<InsertClientResponse> InsertClient(InsertClientRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.UserId, out var userId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "UserId must be a valid Guid"));
        }

        if (!Guid.TryParse(request.Id, out var clientId))
        {
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
