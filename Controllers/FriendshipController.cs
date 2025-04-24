using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sigma_backend.Extensions;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Interfaces.Service;
using sigma_backend.Mappers;
using sigma_backend.Models;

namespace sigma_backend.Controllers
{
    [Route("api/friendships")]
    [Authorize]
    [ApiController]
    public class FriendshipController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFriendRequestService _friendRequestService;
        private readonly IFriendRequestRepository _friendRequestRepo;
        private readonly IFriendshipRepository _friendshipRepo;
        private readonly IPathService _pathService;
        public FriendshipController(
            UserManager<User> userManager,
            ICurrentUserService currentUserService,
            IFriendRequestService friendRequestService,
            IFriendRequestRepository friendRequestRepo,
            IFriendshipRepository friendshipRepo,
            IPathService pathService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
            _friendRequestService = friendRequestService;
            _friendRequestRepo = friendRequestRepo;
            _friendshipRepo = friendshipRepo;
            _pathService = pathService;
        }
        [HttpPost("{username}/request")]
        public async Task<IActionResult> SendFriendRequest(string username)
        {
            // Try to get target users
            var (sender, receiver, result) = await _friendRequestService.GetUsersToTargetAsync(User, username);

            if (result != null)
                return result;

            if (sender == receiver)
                return BadRequest("Users cannot add to friends themselves.");

            if ((await _friendRequestRepo.GetAsync(sender!.Id, receiver!.Id)) != null)
                return BadRequest("Friend request already exist.");

            if ((await _friendshipRepo.GetAsync(sender!.Id, receiver.Id)) != null)
                return BadRequest("User is already your friend.");

            // Check if sender already receiver friend request from receiver
            var counterRequest = await _friendRequestRepo.GetAsync(receiver!.Id, sender!.Id);

            // If counter request exist, accept it
            if (counterRequest != null)
                return await AcceptFriendRequest(receiver.UserName!);

            var request = (await _friendRequestRepo.CreateAsync(sender.Id, receiver.Id))
                .ToFriendRequestDto();

            return Ok(request);
        }
        [HttpDelete("{username}/cancel")]
        public async Task<IActionResult> CancelFriendRequest(string username)
        {
            // Try to get target users
            var (sender, receiver, result) = await _friendRequestService.GetUsersToTargetAsync(User, username);

            if (result != null)
                return result;

            // Try to get friend request
            var request = await _friendRequestRepo.DeleteAsync(sender!.Id, receiver!.Id);

            if (request == null)
                return NotFound();

            return NoContent();
        }
        [HttpPost("{username}/decline")]
        public async Task<IActionResult> DeclineFriendRequest(string username)
        {
            // Try to get target users
            var (sender, receiver, result) = await _friendRequestService.GetUsersFromTargetAsync(User, username);

            if (result != null)
                return result;

            // Try to get friend request
            var request = await _friendRequestRepo.DeleteAsync(sender!.Id, receiver!.Id);

            if (request == null)
                return NotFound();

            return NoContent();
        }
        [HttpGet("sent-requests")]
        public async Task<IActionResult> GetSentRequests()
        {
            // Get the current logged-in user
            var sender = await _currentUserService.GetCurrentUserAsync(User);

            if (sender == null)
                return Unauthorized();

            // Get all requests and convert them to dtos
            var requests = (await _friendRequestRepo.GetSendedFriendRequests(sender.Id))
                .Select(r => r.ToFriendRequestDto()).ToList();

            return Ok(requests);
        }
        [HttpGet("requests")]
        public async Task<IActionResult> GetReceivedRequests()
        {
            // Get the current logged-in user
            var receiver = await _currentUserService.GetCurrentUserAsync(User);

            if (receiver == null)
                return Unauthorized();

            // Get all requests and convert them to dtos
            var requests = (await _friendRequestRepo.GetReceivedFriendRequests(receiver.Id))
                .Select(r => r.ToFriendRequestDto()).ToList();

            return Ok(requests);
        }
        [HttpPost("{username}/accept")]
        public async Task<IActionResult> AcceptFriendRequest(string username)
        {
            // Try to get target users
            var (sender, receiver, result) = await _friendRequestService.GetUsersFromTargetAsync(User, username);

            if (result != null)
                return result;

            // Try to find friend request
            var request = await _friendRequestRepo.DeleteAsync(sender!.Id, receiver!.Id);

            if (request == null)
                return NotFound();

            await _friendshipRepo.CreateAsync(receiver.Id, sender.Id);

            return Ok();
        }
        [HttpGet("/api/profile/me/friends")]
        public async Task<IActionResult> GetFriends()
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user?.UserName == null)
                return Unauthorized();

            return await GetUserFriends(user.UserName, true);
        }
        [HttpGet("/api/profile/{username}/friends")]
        public async Task<IActionResult> GetUserFriends(string username, bool force = false)
        {
            // Get the current logged-in user
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return NotFound();

            if (!user.Profile!.FriendsVisible && !force)
                return Unauthorized();

            // Get all friends and convert them to dtos
            var friends = (await _friendshipRepo.GetFriends(user.Id))
                .Select(f => f!.ToUserSummaryDto(f!.GetProfilePictureUrl(Request, _pathService))).ToList();

            return Ok(friends);
        }
        [HttpGet("friends/{username}")]
        public async Task<IActionResult> GetFriendship(string username)
        {
            // Try to get target users
            var (user, friend, result) = await _friendRequestService.GetUsersToTargetAsync(User, username);

            if (result != null)
                return result;

            // Try to get friendship
            var friendship = await _friendshipRepo.GetAsync(user!.Id, friend!.Id);

            if (friendship == null)
                return NotFound();

            return Ok(friendship.ToFriendshipDto());
        }
        [HttpDelete("friends/{username}")]
        public async Task<IActionResult> RemoveFriend(string username)
        {
            // Try to get target users
            var (user, friend, result) = await _friendRequestService.GetUsersToTargetAsync(User, username);

            if (result != null)
                return result;

            // Try to get friendship
            var request = await _friendshipRepo.DeleteAsync(user!.Id, friend!.Id);

            if (request == null)
                return NotFound();

            return NoContent();
        }
    }
}