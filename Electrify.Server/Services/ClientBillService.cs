using Electrify.Protos;
using Electrify.Server.Services.Abstraction;
using Grpc.Core;

namespace Electrify.Server.Services;

public class ClientBillService(IReadingService readingService) : ClientBills.ClientBillsBase
{
    public override Task<GetClientBillResponse> GetClientBill(GetClientBillRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.ClientId, out var clientId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Client ID must be in GUID format"));
        }
        
        if (DateOnly.TryParseExact(request.Date,"dd/MM/yyyy", out var billDate) == false)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Bill date must be in DateOnly format"));
        }

        double bill = readingService.GetReadingsForDay(clientId, billDate);

        return Task.FromResult(
            new GetClientBillResponse
            {
                PayableAmount = bill
            });
    }
}