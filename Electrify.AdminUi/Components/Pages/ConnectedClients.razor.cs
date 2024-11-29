using Electrify.AdminUi.Services.Abstractions;
using Microsoft.AspNetCore.Components;

namespace Electrify.AdminUi.Components.Pages;

public partial class ConnectedClients : ComponentBase, IDisposable
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IConnectedClientsService ConnectedClientsService { get; set; } = default!;

    private IEnumerable<string> _connectedClientIds = [];
    private Timer? _updateConnectedClientsTimer;

    protected override void OnInitialized()
    {
        _updateConnectedClientsTimer = new Timer(
            UpdateConnectedClientIds,
            null,
            TimeSpan.Zero,
            TimeSpan.FromMilliseconds(200));
    }

    private void ReturnHome()
    {
        NavigationManager.NavigateTo("/home");    
    }
    
    private void GoToBill(string clientId)
    {
        NavigationManager.NavigateTo($"/bill/{clientId}");
    }

    private async void UpdateConnectedClientIds(object? state)
    {
        _connectedClientIds = await ConnectedClientsService.GetConnectedClientIds();
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        _updateConnectedClientsTimer?.Dispose();
        GC.SuppressFinalize(this);
    }
}