using sigma_backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sigma_backend.Interfaces.Repository
{
    public interface IMessageRepository
    {
        Task<Message> SaveMessage(Message message);
        Task<List<Message>> GetMessagesBetweenUsersAsync(string username1, string username2);
    }
}