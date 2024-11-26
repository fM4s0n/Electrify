using Electrify.Models;
using Electrify.Server.ApiClient;
using Electrify.Server.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Electrify.ComponentTests;

public sealed class TestFixture : WebApplicationFactory<Program>
{
    public ElectrifyApiClient ApiClient { get; }
    public ElectrifyDbContext Database { get; }
    
    public ILogger<ElectrifyApiClient> Logger { get; }

    private readonly IServiceScope _serviceScope;
    
    public TestFixture()
    {
        Logger = Substitute.For<ILogger<ElectrifyApiClient>>();
        
        ApiClient = new ElectrifyApiClient(CreateClient(), Logger);

        _serviceScope = Services.CreateScope();
        Database = _serviceScope.ServiceProvider.GetRequiredService<ElectrifyDbContext>();
    }

    public Client CreateEClient()
    {
        Client client = new Client
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
        };
        
        Database.Clients.Add(client);
        
        Database.SaveChanges();

        return client;
    }
    
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        ApiClient.Dispose();
        _serviceScope.Dispose();
    }
}