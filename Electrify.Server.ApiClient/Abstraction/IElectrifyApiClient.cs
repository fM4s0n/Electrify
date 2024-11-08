using Electrify.DlmsServer;
using Electrify.Server.ApiClient.Contracts;

namespace Electrify.Server.ApiClient.Abstraction;

public interface IElectrifyApiClient
{
    Task<AvailabilityResponse> Register(int port, string secret);

    Task<HttpAdminLoginResponse> AdminLogin(string email, string password);
}