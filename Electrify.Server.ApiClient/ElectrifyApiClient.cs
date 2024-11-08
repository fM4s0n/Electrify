using System.Net.Http.Json;
using Electrify.DlmsServer;
using Electrify.Server.ApiClient.Abstraction;
using Electrify.Server.ApiClient.Contracts;
using Electrify.Server.Protos;

namespace Electrify.Server.ApiClient;

public sealed class ElectrifyApiClient(HttpClient httpClient) : IElectrifyApiClient
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
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        var availabilityResponse = await response.Content.ReadFromJsonAsync<AvailabilityResponse>();

        if (availabilityResponse is null)
        {
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
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        var adminLoginResponse = await response.Content.ReadFromJsonAsync<HttpAdminLoginResponse>();

        if (adminLoginResponse is null)
        {
            throw new Exception($"An error occured parsing the {nameof(HttpAdminLoginResponse)}");
        }
        
        return adminLoginResponse;
    }
}