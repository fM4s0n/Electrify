using Electrify.Server.ApiClient;
using Electrify.Server.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Electrify.ComponentTests;

public sealed class TestFixture : WebApplicationFactory<Program>
{
    public ElectrifyApiClient ApiClient { get; }
    public ElectrifyDbContext Database { get; }

    private readonly IServiceScope _serviceScope;
    
    public TestFixture()
    {
        ApiClient = new ElectrifyApiClient(CreateClient());

        _serviceScope = Services.CreateScope();
        Database = _serviceScope.ServiceProvider.GetRequiredService<ElectrifyDbContext>();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        ApiClient.Dispose();
        _serviceScope.Dispose();
    }
}