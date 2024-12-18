using Electrify.Protos;
using Electrify.Server.ApiClient.Contracts;

namespace Electrify.Server.ApiClient.Abstraction;

public interface IElectrifyApiClient : IDisposable
{
    Task<HttpAvailabilityResponse> Register(int port, string secret, Guid clientId);

    Task<HttpAdminLoginResponse> AdminLogin(string email, string password);

    Task<HttpInsertClientResponse> InsertClient(string token, Guid id, Guid userId);

    Task ErrorMessage();

    Task<IEnumerable<string>> GetConnectedClientIds(string token);

    Task<double?> GetClientBill(string clientId, DateOnly date);
}