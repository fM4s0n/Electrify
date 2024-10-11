using Electrify.AdminUi.Services.Abstractions;

namespace Electrify.AdminUi.Services;

public class LoginService : ILoginService
{
    public async Task<bool> ValidateLogin(string email, string password)
    {
        return true;
    }
}
