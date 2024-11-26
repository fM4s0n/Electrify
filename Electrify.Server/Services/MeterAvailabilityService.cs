using Electrify.Dlms.Client;
using Electrify.Dlms.Options;
using Electrify.Protos;
using Electrify.Server.Database;
using Electrify.Server.Options;
using Electrify.Server.Services.Abstraction;
using Grpc.Core;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Secure;
using Gurux.Net;
using Microsoft.Extensions.Options;

namespace Electrify.Server.Services;

public class MeterAvailabilityService(
    IOptions<DlmsClientOptions> dlmsClientOptions,
    IOptions<ObservabilityOptions> observabilityOptions,
    ILogger<DlmsClient> dlmsClientLogger,
    ILogger<MeterAvailabilityService> logger,
    TimeProvider timeProvider,
    ElectrifyDbContext dbContext,
    IDlmsClientService dlmsClientService)
    : MeterAvailability.MeterAvailabilityBase
{
    public override Task<AvailabilityResponse> Register(AvailabilityRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.ClientId, out var clientId))
        {
            logger.LogError("ClientId was not a valid Guid : {ClientId}", request.ClientId);
            throw new RpcException(new Status(StatusCode.InvalidArgument, "ClientId should be in GUID format"));
        }
        
        if (dbContext.Clients.Any(client => client.Id == clientId) == false)
        {
            logger.LogWarning("Client ID is not registered: {ClientId}", request.ClientId);
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Client ID is not registered"));
        }
        
        var media = new GXNet(dlmsClientOptions.Value.Protocol, dlmsClientOptions.Value.ServerHostname, request.Port);
        
        var secureClient = new GXDLMSSecureClient(
            dlmsClientOptions.Value.UseLogicalNameReferencing,
            dlmsClientOptions.Value.ClientAddress,
            dlmsClientOptions.Value.ServerAddress,
            dlmsClientOptions.Value.Authentication,
            request.Secret,
            dlmsClientOptions.Value.InterfaceType);

        var reader = new GXDLMSReader(secureClient, media, observabilityOptions.Value.TraceLevel, dlmsClientOptions.Value.InvocationCounter);

        var registers = dlmsClientOptions.Value.LogicalNames
            .Select(register => new GXDLMSRegister(register));

        try
        {
            var client = new DlmsClient(clientId, dlmsClientLogger, media, reader, registers, timeProvider);

            dlmsClientService.AddClient(request.Port, clientId, client);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An error occured creating a DLMS client on port: {Port}", request.Port);
            
            return Task.FromResult(new AvailabilityResponse
            {
                Success = false,  // TODO maybe we want to display something on MeterUI if this occurs
            });
        }
        
        return Task.FromResult(new AvailabilityResponse
        {
            Success = true,
        });
    }
}
