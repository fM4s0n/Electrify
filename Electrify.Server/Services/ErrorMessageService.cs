using Electrify.Protos;
using Electrify.Server.Services.Abstraction;
using Grpc.Core;

namespace Electrify.Server.Services;

public class ErrorMessageService(IDlmsClientService dlmsClientService) : ErrorMessage.ErrorMessageBase
{
    private readonly Random _random = new();
    
    public override Task<ErrorMessageResponse> DisplayErrorMessage(ErrorMessageRequest request, ServerCallContext context)
    {
        // TODO decide if we want to allow custom errors from API or we should just randomise from set
        string[] errors = ["Example Error A From Grid", "Example Error B From Grid", "Example Error C From Grid"];
        
        foreach (var dlmsClient in dlmsClientService.GetClients())
        {
            dlmsClient.WriteErrorMessage("We have detected a power outage in your area and are working to fix the issue.");
        }

        return Task.FromResult(new ErrorMessageResponse() { Success = true});
    }
}