using Electrify.AdminUi.Services.Abstractions;
using Electrify.Models.Models;
using Microsoft.AspNetCore.Components;

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

    protected override void OnInitialized()
    {
        if (AdminService.CurrentAdmin == null)
        {
            NavigationManager.NavigateTo("/");
        }

        _admin = AdminService.CurrentAdmin;

        base.OnInitialized();
    }

    public void HandleSetupMeter()
    {
        var newCleint = new Client
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
        };
        
        ClientService.InsertClient(newCleint);
        _currentClient = newCleint;
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
}

