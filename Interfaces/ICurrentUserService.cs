using System.Security.Claims;
using sigma_backend.Models;

namespace sigma_backend.Interfaces
{
    public interface ICurrentUserService
    {
        Task<User?> GetCurrentUserAsync(ClaimsPrincipal user);
    }
}