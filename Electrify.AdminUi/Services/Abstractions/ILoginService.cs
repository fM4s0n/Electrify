namespace Electrify.AdminUi.Services.Abstractions;

public interface ILoginService
{
    Task<bool> ValidateLogin(string email, string password);
}
