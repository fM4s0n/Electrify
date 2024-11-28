using System;
using System.Threading;
using System.Threading.Tasks;
using Electrify.Dlms.Options;
using Electrify.Dlms.Server.Abstraction;
using Electrify.Server.ApiClient;
using Electrify.SmartMeterUi.Services.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Electrify.SmartMeterUi.Services;

public class ConnectionService(IHttpClientFactory clientFactory, IOptions<DlmsServerOptions> options, IDlmsServer dlmsServer, IErrorMessageService errorMessageService, ILogger<ElectrifyApiClient> logger) : IConnectionService
{
    private ElectrifyApiClient _apiClient = default!;
    private CancellationTokenSource? _reconnectCts;
    public bool InitialConnectionMade { get; private set; }

    public async Task InitializeConnectionAsync(bool isReconnect)
    {
        // Initialize API client
        _apiClient = new ElectrifyApiClient(clientFactory.CreateClient("ElectrifyServer"), logger);

        // Check if we are retrying a lost connection, only register the DLMS Server if it's the first run
        if (!isReconnect)
        {
            await Task.WhenAll(InitDlmsServer(), RegisterConnectionWithServer());
        }
        // If it's a retry, just connect to the server
        else
        {
            await RegisterConnectionWithServer();
        }
    }

    private async Task InitDlmsServer()
    {
        // Initialize the DLMS server and manage connection state
        await Task.Run(() =>
        {
            dlmsServer.Initialise(
                options,
                () => errorMessageService.IsConnected = true,
                () => errorMessageService.IsConnected = false,
                () => dlmsServer.GetErrorMessage()
            );
        });
    }

    private async Task RegisterConnectionWithServer()
    {
        if (!Guid.TryParse("4b34de2e-c340-4aec-84bf-636e7a388410", out var clientId))
        {
            throw new ArgumentException("Invalid ClientId specified in command line arguments");
        }
        
        await _apiClient.Register(options.Value.Port, options.Value.Password, clientId);
        InitialConnectionMade = true;
    }

    public async Task AttemptReconnectAsync()
    {
        CancelReconnect();

        _reconnectCts = new CancellationTokenSource();
        CancellationToken token = _reconnectCts.Token;

        try
        {
            await Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        Console.WriteLine("Attempting to reconnect...");
                        await InitializeConnectionAsync(true);

                        if (errorMessageService.IsConnected)
                        {
                            Console.WriteLine("Reconnection successful.");
                            CancelReconnect();
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Reconnection attempt failed: {ex.Message}");
                    }

                    await Task.Delay(5000, token);
                }
            }, token);
        }
        catch (TaskCanceledException)
        {
            // Task was canceled
            Console.WriteLine("Reconnection task was canceled.");
        }
        finally
        {
            CancelReconnect();
        }
    }


    private void CancelReconnect()
    {
        if (_reconnectCts == null || _reconnectCts.IsCancellationRequested) return;
        
        _reconnectCts.Cancel();
        _reconnectCts.Dispose();
        _reconnectCts = null;
        Console.WriteLine("Reconnection attempts canceled.");
    }
}
