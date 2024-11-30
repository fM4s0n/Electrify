using Electrify.Server.ApiClient.Contracts;

namespace Electrify.AdminUi.Services.Abstractions;

public interface IAdminService
{
    HttpAdminLoginResponse? CurrentAdmin { get; }
    Task<bool> ValidateLogin(string email, string password);
    void LogoutCurrentAdmin();
}
