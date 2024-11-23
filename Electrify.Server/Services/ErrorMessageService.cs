﻿using Electrify.Protos;
using Electrify.Server.Services.Abstraction;
using Grpc.Core;

namespace Electrify.Server.Services;

public class ErrorMessageService(IDlmsClientService dlmsClientService) : ErrorMessage.ErrorMessageBase
{
    private readonly Random _random = new();
    
    public override async Task<ErrorMessageResponse> DisplayErrorMessage(ErrorMessageRequest request, ServerCallContext context)
    {
        // TODO decide if we want to allow custom errors from API or we should just randomise from set
        string[] errors = ["Example Error A From Grid", "Example Error B From Grid", "Example Error C From Grid"];
        
        foreach (var dlmsClient in dlmsClientService.GetClients())
        {
            var randomError = errors[_random.Next(0, errors.Length)];
            dlmsClient.WriteErrorMessage(randomError);
        }

        return new ErrorMessageResponse
        {
            Success = true
        };
    }
}