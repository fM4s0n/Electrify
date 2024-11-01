﻿using Electrify.Models.Models;
using Electrify.Server.Protos;
using Electrify.Server.Services.Abstraction;
using Grpc.Core;

namespace Electrify.Server.Services;

public class AdminLoginService(IAdminService adminService) : AdminLogin.AdminLoginBase
{
    public override Task<AdminLoginResponse> AdminLogin(AdminLoginDetailsRequest request, ServerCallContext context)
    {
        Admin? admin = adminService.GetAdminByEmail(request.Email);

        if (admin == null || !adminService.VerifyPassword(admin, request.Password))
        {
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
            PasswordHash = admin.PasswordHash,
            Token = admin.AccessToken.ToString(),
        });
    }
}
