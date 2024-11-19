using Electrify.Models;

namespace Electrify.AdminUi.Services.Abstractions;

public interface IAdminService
{
    Admin? CurrentAdmin { get; }
    Task<bool> ValidateLogin(string email, string password);
    void LogoutCurrentAdmin();
}
