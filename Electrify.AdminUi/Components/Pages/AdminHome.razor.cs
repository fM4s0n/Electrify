using Electrify.AdminUi.Services.Abstractions;
using Electrify.Models;
using Microsoft.AspNetCore.Components;
using Electrify.Models.Enums;

namespace Electrify.AdminUi.Components.Pages;

public partial class AdminHome : ComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IAdminService AdminService { get; set; } = default!;

    [Inject]
    private IClientService ClientService { get; set; } = default!;

    [Inject] IGreetingService GreetingService { get; set; } = default!;

    private Admin? _admin;
    private Client? _currentClient;
    private readonly List<ToastDefinition> _toasts = [];
    private Timer? _toastTimer;

    protected override void OnInitialized()
    {
        if (AdminService.CurrentAdmin == null)
        {
            NavigationManager.NavigateTo("/");
        }

        _admin = AdminService.CurrentAdmin;

        base.OnInitialized();
    }

    public async Task HandleSetupMeter()
    {
        var newClient = new Client
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
        };
        
        if (await ClientService.InsertClient(newClient) == false)
        {
            _toasts.Add(new ToastDefinition
            {
                Title = "Error",
                Message = "Failed to setup meter",
                Type = ToastType.InsertClientError,
            });

            if (_toastTimer == null)
            {
                StartToastTimer();
            }
        }

        _currentClient = newClient;
    }

    public void HandleLogout()
    {
        AdminService.LogoutCurrentAdmin();
        NavigationManager.NavigateTo("/");
    }

    private string GetGreeting() => GreetingService.GetGreeting(_admin?.Name ?? "Admin");

    private static async Task CopyToClipboard(string stringToCopy)
    {
        await Clipboard.SetTextAsync(stringToCopy);
    }

    private void HandleNextMeter()
    {
        _currentClient = null;
    }

    private void StartToastTimer()
    {
        if (_toastTimer == null)
        {
            _toastTimer = new Timer(ToastTimerCallback, null, 5000, Timeout.Infinite);
        }
        else
        {
            _toastTimer.Change(5000, Timeout.Infinite);
        }
    }

    private void StopToastTimer()
    {
        _toastTimer?.Dispose();
        _toastTimer = null;
    }

    private async void ToastTimerCallback(object? state)
    {
        if (_toasts.Count > 0)
        {
            _toasts.RemoveAt(0);
            await InvokeAsync(StateHasChanged);

            if (_toasts.Count > 0)
            {
                // Restart the timer for the next toast
                _toastTimer?.Change(5000, Timeout.Infinite);
                return;
            }
  
            StopToastTimer();            
        }
    }
}

