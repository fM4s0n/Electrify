using Electrify.DlmsServer.Database;
using Electrify.DlmsServer.Services;
using Electrify.DlmsServer.Services.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<ElectrifyDbContext>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcSwagger().AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Electrify Authentication",
        Version = "v1",
    });
});

builder.Services.AddDbContext<ElectrifyDbContext>(options => 
    options.UseInMemoryDatabase("ElectrifyDB"));

var app = builder.Build();

app.UseSwagger().UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Electrify Authentication v1");
});

app.MapGrpcService<AuthenticationService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();