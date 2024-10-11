using Electrify.Models.Models;

namespace Electrify.Server.Services.Abstraction;
public interface IAdminService
{
    Admin CreateAdmin(string name, string email, string plainTextPassword);

    bool VerifyPassword(Admin admin, string plainTextPassword);

    Guid GenerateAccessToken();
}