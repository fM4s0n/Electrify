using System.Net.Http.Json;
using Electrify.DlmsServer;
using Electrify.Server.ApiClient.Abstraction;
using Electrify.Server.ApiClient.Contracts;
using Microsoft.Extensions.Logging;

namespace Electrify.Server.ApiClient;

public sealed class ElectrifyApiClient(HttpClient httpClient, ILogger<ElectrifyApiClient> _logger) : IElectrifyApiClient
{
    public async Task<AvailabilityResponse> Register(int port, string secret)
    {
        var response = await httpClient.PostAsJsonAsync("/v1/available", new AvailabilityRequest
        {
            Port = port,
            Secret = secret,
        });

        if (!response.IsSuccessStatusCode)
        {
            _logger
                .LogError(
                    "Failed to register the server with the Electrify API. Response: {Response}",
                    response);
            
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        var availabilityResponse = await response.Content.ReadFromJsonAsync<AvailabilityResponse>();

        if (availabilityResponse is null)
        {
            _logger.LogError("Failed to parse the {AvailabilityResponse}", availabilityResponse);
            
            throw new Exception($"An error occured parsing the {nameof(AvailabilityResponse)}");
        }
        
        return availabilityResponse;
    }

    public async Task<HttpAdminLoginResponse> AdminLogin(string email, string password)
    {
        var response = await httpClient.PostAsJsonAsync("/v1/adminLogin", new HttpAdminLoginRequest
        {
            Email = email,
            Password = password,
        });

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to login the admin. Response: {Response}", response);
            
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        var adminLoginResponse = await response.Content.ReadFromJsonAsync<HttpAdminLoginResponse>();

        if (adminLoginResponse is null)
        {
            _logger.LogError("Failed to parse the {HttpAdminLoginResponse}", adminLoginResponse);
            
            throw new Exception($"An error occured parsing the {nameof(HttpAdminLoginResponse)}");
        }
        
        return adminLoginResponse;
    }

    public async Task<HttpInsertClientResponse> InsertClient(Guid id, Guid userId)
    {
        var response = await httpClient.PostAsJsonAsync("/v1/insertClient", new HttpInsertClientRequest
        {
            Id = id.ToString(),
            UserId = userId.ToString(),
        });

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to insert the client. Response: {Response}", response);
            
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        var insertClientResponse = await response.Content.ReadFromJsonAsync<HttpInsertClientResponse>();

        if (insertClientResponse is null)
        {
            _logger.LogError("Failed to parse the {HttpInsertClientResponse}", insertClientResponse);
            
            throw new Exception($"An error occured parsing the {nameof(HttpInsertClientResponse)}");
        }

        return insertClientResponse;
    }
}