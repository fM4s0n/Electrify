using Electrify.DlmsServer;
using Electrify.Server.Protos;

namespace Electrify.Server.ApiClient.Abstraction;

public interface IElectrifyApiClient
{
    Task<AvailabilityResponse> Register(int port, string secret);
    Task<AdminLoginResponse> AdminLogin(string email, string password);
}