using Electrify.Dlms.Extensions;
using Electrify.Dlms.Options;
using Electrify.Server.Database;
using Electrify.Server.Interceptors;
using Electrify.Server.Options;
using Electrify.Server.Services;
using Electrify.Server.Services.Abstraction;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

if (!Enum.TryParse(builder.Configuration["Serilog:MinimumLevel"], out LogLevel logLevel))
{
    Environment.FailFast("Log Level has not been set");
}

builder.Services.AddSingleton(Options.Create(new ObservabilityOptions
{
    LogLevel = logLevel,
    TraceLevel = logLevel.ToTraceLevel(),
}));

builder.Services.AddHttpClient(
    "OctopusClient",
    client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["OctopusApiUrl"]!);
    });

builder.Services.Configure<DlmsClientOptions>(builder.Configuration.GetSection(nameof(DlmsClientOptions)));
builder.Services.Configure<TariffOptions>(builder.Configuration.GetSection(nameof(TariffOptions)));

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddDbContext<ElectrifyDbContext>(options => 
    options.UseInMemoryDatabase("ElectrifyDB"));

builder.Services.AddScoped(typeof(PasswordHasher<>));
builder.Services.AddScoped<IReadingService, ReadingService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

builder.Services.AddSingleton<IOctopusService, OctopusService>();

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<AuthenticationInterceptor>();
}).AddJsonTranscoding();

builder.Services.AddGrpcSwagger().AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Electrify",
        Version = "v1",
    });
});

builder.Services.AddSingleton<IDlmsClientService, DlmsClientService>();
builder.Services.AddHostedService<TariffService>();

var app = builder.Build();

app.UseSwagger().UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Electrify v1");
});

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
    await seeder.SeedDefaultAdmin();
    await seeder.SeedDefaultClientId();
    await seeder.SeedDefaultReadings();
}

app.MapGrpcService<MeterAvailabilityService>();
app.MapGrpcService<AdminLoginService>();
app.MapGrpcService<InsertClientService>();
app.MapGrpcService<ErrorMessageService>();
app.MapGrpcService<ConnectedClientsService>();
app.MapGrpcService<ClientBillService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

// TODO should maybe refactor proto files and to make it so they're excludable from test coverage

namespace Electrify.Server
{
    public partial class Program;
}