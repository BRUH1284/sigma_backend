using sigma_backend.DataTransferObjects.User;
using sigma_backend.Models;

namespace sigma_backend.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> SearchByUsernameAsync(string query);
        Task<User?> GetByUsernameAsync(string username); 
        Task SaveExpoToken(string username, string token);
        Task<List<string>> GetExpoTokens(string username);

    }
}