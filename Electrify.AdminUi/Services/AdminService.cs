using Electrify.AdminUi.Services.Abstractions;
using Electrify.Models.Models;
using Electrify.Server.Protos;

namespace Electrify.AdminUi.Services;

public class AdminService(AdminLogin.AdminLoginClient adminLoginClient) : IAdminService
{
    private readonly AdminLogin.AdminLoginClient _adminLoginClient = adminLoginClient;
    private Admin? CurrentAdmin;

    public Admin? GetCurrentAdmin() => CurrentAdmin;

    public async Task<bool> ValidateLogin(string email, string password)
    {
        var reply = await _adminLoginClient.AdminLoginAsync(new AdminLoginDetailsRequest
        {
            Email = email,
            Password = password,
        });

        if (reply.Success)
        {
            CurrentAdmin = new Admin()
            {
                Id = Guid.NewGuid(),
                Name = reply.Name,
                Email = reply.Email,
                PasswordHash = reply.PasswordHash,
                AccessToken = Guid.Parse(reply.Token)
            };
        }

        return reply.Success;
    }

    public void LogoutCurrentAdmin()
    {
        CurrentAdmin = null;
    }
}
