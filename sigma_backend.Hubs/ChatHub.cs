using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace sigma_backend.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;

        public ChatHub(IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        public async Task SendMessage(string receiverUsername, string message)
        {
            var senderUsername = Context.User?.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(senderUsername) || string.IsNullOrEmpty(receiverUsername))
                throw new HubException("Invalid sender or receiver username");

            var receiver = await _userRepository.GetByUsernameAsync(receiverUsername);
            if (receiver == null)
                throw new HubException("Receiver not found");

            var messageEntity = new Message
            {
                Id = Guid.NewGuid().ToString(),
                SenderUsername = senderUsername,
                ReceiverUsername = receiverUsername,
                Content = message,
                SentAt = DateTime.UtcNow
            };
            await _messageRepository.SaveMessage(messageEntity);

            await Clients.Group($"User_{receiverUsername}").SendAsync("ReceiveMessage", senderUsername, message, messageEntity.SentAt);
        }

        public override async Task OnConnectedAsync()
        {
            var username = Context.User?.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (!string.IsNullOrEmpty(username))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{username}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var username = Context.User?.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (!string.IsNullOrEmpty(username))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{username}");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}