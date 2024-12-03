using Electrify.Dlms.Options;
using Electrify.Dlms.Server.Abstraction;
using Electrify.Server.ApiClient.Abstraction;
using Electrify.SmartMeterUi.Services.Abstraction;
using Microsoft.Extensions.Options;

namespace Electrify.SmartMeterUi.Services;

public class ConnectionService(
    IElectrifyApiClient electrifyApiClient,
    IOptions<DlmsServerOptions> options,
    IDlmsServer dlmsServer,
    IErrorMessageService errorMessageService,
    ICommandLineArgsProvider commandLineArgsProvider,
    IUsageService usageService)
    : IConnectionService
{
    private CancellationTokenSource? _reconnectCts;
    public bool InitialConnectionMade { get; private set; }

    public async Task InitializeConnectionAsync(bool isReconnect)
    {
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
                () => errorMessageService.ErrorMessage = dlmsServer.GetErrorMessage()
            );
        });
    }

    private async Task RegisterConnectionWithServer()
    {
        if (!Guid.TryParse(commandLineArgsProvider.GetArgAtIndex(1), out var clientId))
        {
            throw new ArgumentException("Invalid ClientId specified in command line arguments");
        }
        
        var response = await electrifyApiClient.Register(options.Value.Port, options.Value.Password, clientId);
        
        var readings = response.HistoricReadings
            .Select(hr => (DateTime.Parse(hr.Timestamp), hr.Usage, hr.Tariff))
            .ToList();
        
        if (readings.Count != 0)
        {
            var mostRecentReading = readings.MaxBy(r => r.Item1);
            usageService.SetLastUsage(mostRecentReading.Item1, mostRecentReading.Usage);
            
            dlmsServer.InsertHistoricReadings(readings);
        }
        
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
