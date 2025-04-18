using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

public class AlphanumericUsernameValidator<TUser> : IUserValidator<TUser> where TUser : IdentityUser
{
    private const int MinLength = 5;
    private const int MaxLength = 32;
    private static readonly Regex _validUsernameRegex = new Regex("^[a-z0-9]+$", RegexOptions.Compiled);

    public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
    {
        var errors = new List<IdentityError>();

        if (user.UserName != null)
        {
            if (!_validUsernameRegex.IsMatch(user.UserName))
                errors.Add(new IdentityError
                {
                    Code = "UserNameNotAlphanumeric",
                    Description = "User name can only contain lowercase letters and digits (a-z, 0-9)."
                });

            if (user.UserName.Length < MinLength || user.UserName.Length > MaxLength)
                errors.Add(new IdentityError
                {
                    Code = "UsernameLength",
                    Description = $"Username must be between {MinLength} and {MaxLength} characters."
                });
        }

        return Task.FromResult(errors.Any()
            ? IdentityResult.Failed(errors.ToArray())
            : IdentityResult.Success);
    }
}
