﻿using Electrify.Models;

namespace Electrify.Server.Services.Abstraction;
public interface IAdminService
{
    Task CreateAdmin(string name, string email, string plainTextPassword);

    bool VerifyPassword(Admin admin, string plainTextPassword);

    Guid GenerateAccessToken();

    void UpdateAccessToken(Admin admin, Guid? token);

    Admin? GetAdminByEmail(string email);

    Task<bool> ValidateToken(Guid token);
}