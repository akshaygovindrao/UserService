using Microsoft.AspNetCore.Identity;
using UserService.Domain;

namespace UserService.Security;

public class PasswordHasher : IPasswordHasher
{
    #region Fields

    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<User> _identityHasher = new();

    #endregion

    #region Public Methods

    public string Hash(string password)
    {
        return _identityHasher.HashPassword(new User(), password);
    }

    public bool Verify(string hashedPassword, string providedPassword)
    {
        var result = _identityHasher.VerifyHashedPassword(new User(), hashedPassword, providedPassword);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }

    #endregion
}
