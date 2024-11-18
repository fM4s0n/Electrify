using Electrify.Models;
using Microsoft.EntityFrameworkCore;

namespace Electrify.Server.Database;

/// <summary>
/// DB context for Electrify.
/// </summary>
public class ElectrifyDbContext(DbContextOptions<ElectrifyDbContext> options) : DbContext(options)
{
    public DbSet<Admin> Admins { get; set; }    
    public DbSet<Client> Clients { get; set; }    
    public DbSet<Reading> Readings { get; set; }
    public DbSet<Tariff> Tariffs { get; set; }

    public DateTime? GetLastReading(Guid clientId)
    {
        return Readings.Where(r => r.ClientId == clientId).MaxBy(r => r.DateTime)?.DateTime;
    }
}
