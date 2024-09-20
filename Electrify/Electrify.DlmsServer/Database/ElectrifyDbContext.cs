using Electrify.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Electrify.DlmsServer.Database;

/// <summary>
/// DB context for Electrify.
/// </summary>
public class ElectrifyDbContext(DbContextOptions<ElectrifyDbContext> options) : DbContext(options)
{
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Reading> Readings { get; set; }

}
