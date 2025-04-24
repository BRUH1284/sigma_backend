using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sigma_backend.Interfaces.Service;
using sigma_backend.Models;

namespace sigma_backend.Service
{
    public class FriendRequestService : IFriendRequestService
    {
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public FriendRequestService(
            UserManager<User> userManager,
            ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        public async Task<(User? User, User? Target, IActionResult? ErrorResult)>
            GetUsersToTargetAsync(ClaimsPrincipal userClaims, string username)
        {
            var user = await _currentUserService.GetCurrentUserAsync(userClaims);
            if (user == null)
                return (null, null, new UnauthorizedResult());

            var target = await _userManager.FindByNameAsync(username);
            if (target == null)
                return (null, null, new NotFoundResult());

            return (user, target, null);
        }
        public async Task<(User? Target, User? User, IActionResult? ErrorResult)>
            GetUsersFromTargetAsync(ClaimsPrincipal targetClaims, string username)
        {
            var target = await _currentUserService.GetCurrentUserAsync(targetClaims);
            if (target == null)
                return (null, null, new UnauthorizedResult());

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return (null, null, new NotFoundResult());

            return (user, target, null);
        }
    }
}