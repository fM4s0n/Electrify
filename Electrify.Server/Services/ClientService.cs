using Electrify.Server.Database;
using Electrify.Server.Services.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace Electrify.Server.Services;

public sealed class ClientService(ElectrifyDbContext database) : IClientService
{
    public async Task<bool> ClientExists(Guid userId, Guid clientId)
    {
        return await database.Clients.AnyAsync(c => c.UserId == userId && c.ClientId == clientId);
    }
}