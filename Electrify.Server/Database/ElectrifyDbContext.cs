using Electrify.Models.Models;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>().HasKey(a => a.Id); // is this right?
        modelBuilder.Entity<Client>().HasKey(c => c.UserId);
        modelBuilder.Entity<Reading>().HasKey(r => r.Id);
    }
}
