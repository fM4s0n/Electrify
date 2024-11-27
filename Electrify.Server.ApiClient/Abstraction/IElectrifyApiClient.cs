using Electrify.Protos;
using Electrify.Server.ApiClient.Contracts;

namespace Electrify.Server.ApiClient.Abstraction;

public interface IElectrifyApiClient : IDisposable
{
    Task<AvailabilityResponse> Register(int port, string secret, Guid clientId);

    Task<HttpAdminLoginResponse> AdminLogin(string email, string password);

    Task<HttpInsertClientResponse> InsertClient(Guid id, Guid userId);

    Task ErrorMessage();

    Task<IEnumerable<string>> GetConnectedClientIds(string token);
}