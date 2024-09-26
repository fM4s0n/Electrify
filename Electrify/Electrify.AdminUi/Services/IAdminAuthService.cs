using Electrify.Models.Models;

namespace Electrify.AdminUi.Services;

/// <summary>
/// Auth service for admins.
/// </summary>
internal interface IAdminAuthService
{
    /// <summary>
    /// Verifies the password of the admin account is correct.
    /// </summary>
    /// <param name="admin">Admin account attempting to login</param>
    /// <param name="plainTextPassword">The plain password passed in by the user.</param>
    /// <returns>True if Success or SuccessNeedsRehash, otherwise returns false.</returns>
    bool VerifyPassword(Admin admin, string plainTextPassword);
}
