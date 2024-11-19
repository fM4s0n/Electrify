using Electrify.Dlms.Client.Abstraction;

namespace Electrify.Server.Services.Abstraction;

public interface IDlmsClientService : IAsyncDisposable
{
    IDlmsClient? TryGetClient(int port);

    void AddClient(int port, Guid clientId, IDlmsClient client);
    
    IEnumerable<IDlmsClient> GetClients();
}