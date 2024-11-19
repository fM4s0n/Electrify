using Electrify.Dlms.Extensions;
using Electrify.Dlms.Options;
using Electrify.Server.Database;
using Electrify.Server.Options;
using Electrify.Server.Services;
using Electrify.Server.Services.Abstraction;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddDbContext<ElectrifyDbContext>(options => 
    options.UseInMemoryDatabase("ElectrifyDB"));

builder.Services.AddScoped(typeof(PasswordHasher<>));
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
builder.Services.AddSingleton<IOctopusService, OctopusService>();
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcSwagger().AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Electrify",
        Version = "v1",
    });
});

var app = builder.Build();

app.UseSwagger().UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Electrify v1");
});

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
    seeder.SeedDefaultAdmin();
}

app.MapGrpcService<AuthenticationService>();
app.MapGrpcService<MeterAvailabilityService>();
app.MapGrpcService<AdminLoginService>();
app.MapGrpcService<InsertClientService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();