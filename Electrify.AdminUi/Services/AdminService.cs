﻿using Electrify.AdminUi.Services.Abstractions;
using Electrify.Models.Models;
using Electrify.Server.ApiClient.Abstraction;
using Electrify.Server.Protos;

namespace Electrify.AdminUi.Services;

public class AdminService(IElectrifyApiClient electrifyApiClient) : IAdminService
{
    public Admin? CurrentAdmin { get; private set; }
    
    public async Task<bool> ValidateLogin(string email, string password)
    {
        var response = await electrifyApiClient.AdminLogin(email, password);

        if (response.Success)
        {
            CurrentAdmin = new Admin
            {
                Id = Guid.NewGuid(),
                Name = response.Name,
                Email = response.Email,
                PasswordHash = response.PasswordHash,
                AccessToken = Guid.Parse(response.Token)
            };
        }

        return response.Success;
    }

    public void LogoutCurrentAdmin()
    {
        CurrentAdmin = null;
    }
}
