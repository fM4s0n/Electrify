using Electrify.AdminUi.Services.Abstractions;
using Microsoft.AspNetCore.Components;

namespace Electrify.AdminUi.Components.Pages;

public partial class Login : ComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject] 
    private IAdminService AdminService { get; set; } = default!;

    private readonly LoginDetails LoginDetails = new();

    private bool InvalidLogin = false;

    private async Task HandleLogin()
    {
        bool validLogin = await AdminService.ValidateLogin(LoginDetails.Email, LoginDetails.Password);

        if (validLogin)
        {
            NavigationManager.NavigateTo("/home");
        }
        else
        {
            // Show error message
            InvalidLogin = true;
        }
    }
}

internal class LoginDetails
{
    public string Email = string.Empty;
    public string Password = string.Empty;
}