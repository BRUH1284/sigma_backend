using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_backend.Repository
{
    public class MessageRepository : RepositoryBase, IMessageRepository
    {
        public MessageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Message> SaveMessage(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<List<Message>> GetMessagesBetweenUsersAsync(string username1, string username2)
        {
            return await _context.Messages
                .Where(m => (m.SenderUsername == username1 && m.ReceiverUsername == username2) || 
                           (m.SenderUsername == username2 && m.ReceiverUsername == username1))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<Message> GetLastMessageAsync(string currentUsername, string otherUsername)
        {
            return await _context.Messages
                .Where(m => (m.SenderUsername == currentUsername && m.ReceiverUsername == otherUsername) ||
                            (m.SenderUsername == otherUsername && m.ReceiverUsername == currentUsername))
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<string>> GetConversationUsersAsync(string currentUsername)
        {
            return await _context.Messages
                .Where(m => m.SenderUsername == currentUsername || m.ReceiverUsername == currentUsername)
                .Select(m => m.SenderUsername == currentUsername ? m.ReceiverUsername : m.SenderUsername)
                .Distinct()
                .ToListAsync();
        }
    }
}