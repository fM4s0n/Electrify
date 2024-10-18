using Electrify.Models.Models;

namespace Electrify.AdminUi.Services.Abstractions;

public interface IAdminService
{
    Task<bool> ValidateLogin(string email, string password);

    void LogoutCurrentAdmin();

    Admin? GetCurrentAdmin();
}
