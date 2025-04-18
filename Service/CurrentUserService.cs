using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sigma_backend.Interfaces.Service;
using sigma_backend.Models;

public class CurrentUserService : ICurrentUserService
{
    private readonly UserManager<User> _userManager;

    public CurrentUserService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<User?> GetCurrentUserAsync(ClaimsPrincipal user)
    {
        var userName = user?.Identity?.Name;
        if (string.IsNullOrEmpty(userName)) return null;

        return await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
    }
}
