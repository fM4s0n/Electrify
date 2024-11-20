using Electrify.AdminUi.Services.Abstractions;
using Electrify.Models;
using Electrify.Server.ApiClient.Abstraction;
using Microsoft.Extensions.Logging;

namespace Electrify.AdminUi.Services;

public class AdminService(IElectrifyApiClient electrifyApiClient, ILogger<AdminService> logger) : IAdminService
{
    public Admin? CurrentAdmin { get; private set; }
    
    public async Task<bool> ValidateLogin(string email, string password)
    {
        var response = await electrifyApiClient.AdminLogin(email, password);

        if (response.Success == false)
        {
            logger.LogWarning("Admin login failed for email: {email}", email);
            return false;
        }
        
        CurrentAdmin = new Admin
        {
            Id = Guid.Parse(response.Id),
            Name = response.Name,
            Email = response.Email,
            PasswordHash = response.PasswordHash,
            AccessToken = Guid.Parse(response.Token)
        };

        return true;
    }

    public void LogoutCurrentAdmin() => CurrentAdmin = null;
}
