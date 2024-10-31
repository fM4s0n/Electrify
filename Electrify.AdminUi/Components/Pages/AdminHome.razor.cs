using Electrify.AdminUi.Services.Abstractions;
using Electrify.Models.Models;
using Microsoft.AspNetCore.Components;

namespace Electrify.AdminUi.Components.Pages;

public partial class AdminHome : ComponentBase
{
    [Inject]
    private NavigationManager? NavigationManager { get; set; }

    [Inject]
    private IAdminService? AdminService { get; set; }

    private Admin? _admin;

    private Client? CurrentClient;

    protected override void OnInitialized()
    {
        if (AdminService == null || AdminService.GetCurrentAdmin() == null)
        {
            NavigationManager!.NavigateTo("/");
        }

        _admin = AdminService!.GetCurrentAdmin();

        base.OnInitialized();
    }

    public void HandleSetupMeter()
    {
        CurrentClient = new()
        {
            UserId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
        };

        // TODO: insert into DB
    }

    public void HandleLogout()
    {
        AdminService!.LogoutCurrentAdmin();
        NavigationManager!.NavigateTo("/");
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
        CurrentClient = null;
    }
}

