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
        _currentClient = new Client
        {
            UserId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
        };

        // TODO: insert into DB
    }

    public void HandleLogout()
    {
        AdminService.LogoutCurrentAdmin();
        NavigationManager.NavigateTo("/");
    }

    private string GetGreeting()
    {
        string name = _admin?.Name ?? "Admin";

        string timeOfDayGreeting = DateTime.Now.Hour switch
        {
            < 12 => "Good Morning",
            < 17 => "Good Afternoon",
            _ => "Good Evening"
        };

        return $"{timeOfDayGreeting}, {name}";
    }

    private static async Task CopyToClipboard(string stringToCopy)
    {
        await Clipboard.SetTextAsync(stringToCopy);
    }

    private void HandleNextMeter()
    {
        _currentClient = null;
    }
}

