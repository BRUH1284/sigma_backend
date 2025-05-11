using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using sigma_backend.DataTransferObjects.Message;
using sigma_backend.Hubs;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using System.Security.Claims;

namespace sigma_backend.Controllers
{
    [Route("api/messages")]
    [Authorize]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;

        public MessageController(IHubContext<ChatHub> hubContext, IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _hubContext = hubContext;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var claims = User.Claims;
            var senderUsername = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (claims == null || !claims.Any())
            {
                return BadRequest("No claims found in the token.");
            }
            if (string.IsNullOrEmpty(senderUsername))
            {
                return BadRequest($"Name claim not found. Available claims: {string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}"))}");
            }

            if (string.IsNullOrEmpty(senderUsername) || string.IsNullOrEmpty(dto.ReceiverUsername))
                return BadRequest("Invalid sender or receiver username");

            var receiver = await _userRepository.GetByUsernameAsync(dto.ReceiverUsername);
            if (receiver == null)
                return BadRequest("Receiver not found");

            var messageEntity = new Message
            {
                Id = Guid.NewGuid().ToString(),
                SenderUsername = senderUsername,
                ReceiverUsername = dto.ReceiverUsername,
                Content = dto.Content,
                SentAt = DateTime.UtcNow
            };

            await _messageRepository.SaveMessage(messageEntity);
            await _hubContext.Clients.Group($"User_{dto.ReceiverUsername}").SendAsync("ReceiveMessage", senderUsername, dto.Content, messageEntity.SentAt);

            return Ok();
        }

        [HttpGet("conversation/{otherUsername}")]
        public async Task<IActionResult> GetConversation(string otherUsername)
        {
            var currentUsername = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(currentUsername) || string.IsNullOrEmpty(otherUsername))
                return BadRequest("Invalid username");

            var messages = await _messageRepository.GetMessagesBetweenUsersAsync(currentUsername, otherUsername);
            return Ok(messages);
        }

        [HttpGet("chats")]
        public async Task<IActionResult> GetChats()
        {
            var currentUsername = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(currentUsername))
                return BadRequest("User not found");

            // Получаем пользователей, с которыми есть сообщения
            var users = await _messageRepository.GetConversationUsersAsync(currentUsername);

            // Для каждого пользователя получаем последнее сообщение
            var chats = new List<object>();
            foreach (var user in users)
            {
                var lastMessage = await _messageRepository.GetLastMessageAsync(currentUsername, user);

                chats.Add(new
                {
                    Username = user,
                    LastMessage = lastMessage.Content,
                    SentAt = lastMessage.SentAt
                });
            }

            return Ok(chats);
        }
    }
}