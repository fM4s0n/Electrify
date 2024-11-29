using Electrify.Dlms.Client.Abstraction;

namespace Electrify.Server.Services.Abstraction;

public interface IDlmsClientService : IAsyncDisposable
{
    void AddClient(int port, Guid clientId, IDlmsClient client);
    
    IEnumerable<IDlmsClient> GetClients();

    void TryRemoveClient(Guid clientId);
}