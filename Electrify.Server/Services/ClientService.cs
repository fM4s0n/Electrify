using Electrify.Server.Database;
using Electrify.Server.Services.Abstraction;
using Microsoft.EntityFrameworkCore;
using Electrify.Models;

namespace Electrify.Server.Services;

public sealed class ClientService(ElectrifyDbContext database, ILogger<ClientService> logger) : IClientService
{
    private readonly ILogger<ClientService> _logger = logger;

    public async Task<bool> ClientExists(Guid userId, Guid clientId)
    {
        return await database.Clients.AnyAsync(c => c.UserId == userId && c.Id == clientId);
    }

    public async Task<bool> InsertClient(Client client)
    {
        try
        {
            database.Clients.Add(client);
            await database.SaveChangesAsync();
            _logger.LogInformation("New client inserted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to insert client: {message}", ex.Message);
            return false;
        }

        return true;
    }
}