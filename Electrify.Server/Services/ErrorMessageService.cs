using Electrify.Protos;
using Electrify.Server.Services.Abstraction;
using Grpc.Core;

namespace Electrify.Server.Services;

public class ErrorMessageService(IDlmsClientService dlmsClientService) : ErrorMessage.ErrorMessageBase
{
    private readonly Random _random = new();
    
    public override async Task<ErrorMessageResponse> DisplayErrorMessage(ErrorMessageRequest request, ServerCallContext context)
    {
        foreach (var dlmsClient in dlmsClientService.GetClients())
        {
            dlmsClient.WriteErrorMessage("We have detected a power outage in your area and are working to fix the issue.");
        }

        return new ErrorMessageResponse
        {
            Success = true
        };
    }
}