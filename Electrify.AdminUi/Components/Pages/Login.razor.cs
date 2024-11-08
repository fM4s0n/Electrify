using Electrify.AdminUi.Services.Abstractions;
using Microsoft.AspNetCore.Components;

namespace Electrify.AdminUi.Components.Pages;

public partial class Login : ComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject] 
    private IAdminService AdminService { get; set; } = default!;

    private readonly LoginDetails _loginDetails = new();

    private bool _invalidLogin = false;

    private async Task HandleLogin()
    {
        bool validLogin = await AdminService.ValidateLogin(_loginDetails.Email, _loginDetails.Password);

        if (validLogin)
        {
            NavigationManager.NavigateTo("/home");
        }
        else
        {
            // Show error message
            _invalidLogin = true;
        }
    }
}

internal class LoginDetails
{
    public string Email = string.Empty;
    public string Password = string.Empty;
}