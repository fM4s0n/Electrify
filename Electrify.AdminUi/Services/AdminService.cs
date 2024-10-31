using Electrify.AdminUi.Services.Abstractions;
using Electrify.Models.Models;

namespace Electrify.AdminUi.Services;

public class AdminService : IAdminService
{
    private static Admin? CurrentAdmin;

    public Admin? GetCurrentAdmin() => CurrentAdmin;

    public async Task<bool> ValidateLogin(string email, string password)
    {
        bool valid = true;

        if (valid)
        {
            CurrentAdmin = new Admin()
            {
                Id = Guid.NewGuid(),
                Name = "Freddie",
                Email = "freddie@freddie.com",
                PasswordHash = "password",
                AccessToken = Guid.NewGuid(),
            };
        }

        return valid;
    }

    public void LogoutCurrentAdmin()
    {
        CurrentAdmin = null;
    }
}
