using Electrify.Models;
using Electrify.Server.Services.Abstraction;
using Grpc.Core;
using Electrify.Protos;

namespace Electrify.Server.Services;

public class AdminLoginService(IAdminService adminService, ILogger<AdminLoginService> logger) : AdminLogin.AdminLoginBase
{
    public override Task<AdminLoginResponse> AdminLogin(AdminLoginRequest request, ServerCallContext context)
    {
        Admin? admin = adminService.GetAdminByEmail(request.Email);

        if (admin == null || !adminService.VerifyPassword(admin, request.Password))
        {
            logger.LogWarning("Invalid login attempt for email: {Email}", request.Email);
            
            return Task.FromResult(new AdminLoginResponse
            {
                Success = false,
            });
        }

        admin.AccessToken = adminService.GenerateAccessToken();
        adminService.UpdateAccessToken(admin, admin.AccessToken);

        return Task.FromResult(new AdminLoginResponse
        {
            Success = true,
            Id = admin.Id.ToString(),
            Name = admin.Name,
            Email = admin.Email,
            Token = admin.AccessToken.ToString(),
        });
    }
}
