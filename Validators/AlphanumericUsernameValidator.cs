using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

public class AlphanumericUsernameValidator<TUser> : IUserValidator<TUser> where TUser : IdentityUser
{
    private static readonly Regex _validUsernameRegex = new Regex("^[a-z0-9]+$", RegexOptions.Compiled);

    public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
    {
        var errors = new List<IdentityError>();

        if (user.UserName != null && !_validUsernameRegex.IsMatch(user.UserName))
        {
            errors.Add(new IdentityError
            {
                Code = "UserNameNotAlphanumeric",
                Description = "User name can only contain lowercase letters and digits (a-z, 0-9)."
            });
        }

        return Task.FromResult(errors.Any()
            ? IdentityResult.Failed(errors.ToArray())
            : IdentityResult.Success);
    }
}
