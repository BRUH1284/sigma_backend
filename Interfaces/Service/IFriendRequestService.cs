using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using sigma_backend.Models;

namespace sigma_backend.Interfaces.Service
{
    public interface IFriendRequestService
    {
        Task<(User? User, User? Target, IActionResult? ErrorResult)>
        GetUsersToTargetAsync(ClaimsPrincipal userClaims, string username);
        Task<(User? Target, User? User, IActionResult? ErrorResult)>
        GetUsersFromTargetAsync(ClaimsPrincipal targetClaims, string username);
    }

}