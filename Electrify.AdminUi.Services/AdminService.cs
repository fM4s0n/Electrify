using Electrify.AdminUi.Services.Abstractions;
using Electrify.Server.ApiClient.Abstraction;
using Electrify.Server.ApiClient.Contracts;

namespace Electrify.AdminUi.Services;

public class AdminService(IElectrifyApiClient electrifyApiClient) : IAdminService
{
    public HttpAdminLoginResponse? CurrentAdmin { get; private set; }
    
    public async Task<bool> ValidateLogin(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));
        }

        var response = await electrifyApiClient.AdminLogin(email, password);

        if (response?.Success == true)
        {
            CurrentAdmin = response;
        }

        return response?.Success ?? false;
    }

    public void LogoutCurrentAdmin() => CurrentAdmin = null;
}
