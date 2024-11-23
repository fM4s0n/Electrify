using Electrify.AdminUi.Services.Abstractions;
using Electrify.Models;
using Electrify.Server.ApiClient.Abstraction;

namespace Electrify.AdminUi.Services;

public class AdminService(IElectrifyApiClient electrifyApiClient) : IAdminService
{
    public Admin? CurrentAdmin { get; private set; }
    
    public async Task<bool> ValidateLogin(string email, string password)
    {
        var response = await electrifyApiClient.AdminLogin(email, password);

        if (response.Success)
        {
            CurrentAdmin = new Admin
            {
                Id = Guid.Parse(response.Id),
                Name = response.Name,
                Email = response.Email,
                PasswordHash = string.Empty,
                AccessToken = Guid.Parse(response.Token)
            };
        }

        return response.Success;
    }

    public void LogoutCurrentAdmin() => CurrentAdmin = null;
}
