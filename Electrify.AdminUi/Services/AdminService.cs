using Electrify.AdminUi.Services.Abstractions;
using Electrify.Models.Models;
using Electrify.Server.Protos;

namespace Electrify.AdminUi.Services;

public class AdminService(AdminLogin.AdminLoginClient adminLoginClient) : IAdminService
{
    public Admin? CurrentAdmin { get; private set; }
    
    public async Task<bool> ValidateLogin(string email, string password)
    {
        var reply = await adminLoginClient.AdminLoginAsync(new AdminLoginRequest
        {
            Email = email,
            Password = password,
        });

        if (reply.Success)
        {
            CurrentAdmin = new Admin
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
