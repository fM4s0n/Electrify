using Electrify.DlmsServer.Database;
using Electrify.DlmsServer.Services.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace Electrify.DlmsServer.Services;

public sealed class ClientService(ElectrifyDbContext database) : IClientService
{
    public async Task<bool> ClientExists(Guid userId, Guid clientId)
    {
        return await database.Clients.AnyAsync(c => c.UserId == userId && c.ClientId == clientId);
    }
}