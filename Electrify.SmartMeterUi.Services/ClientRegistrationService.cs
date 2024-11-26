using Electrify.Server.ApiClient.Abstraction;
using Electrify.SmartMeterUi.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace Electrify.SmartMeterUi.Services;

public class ClientRegistrationService(IElectrifyApiClient electrifyApiClient, ILogger<ClientRegistrationService> logger) : IClientRegistrationService
{
    public async Task<bool> ClientIdExists(Guid clientId)
    {
        if (clientId == Guid.Empty)
        {
            throw new ArgumentException("Value cannot be Guid.Empty.", nameof(clientId));
        }

        var response = await electrifyApiClient.ClientIdExists(clientId);

        if (response is null)
        {
            logger.LogWarning("Failed to check if client ID exists: {clientId}", clientId);
        }

        return response?.Success ?? false;
    }
}
