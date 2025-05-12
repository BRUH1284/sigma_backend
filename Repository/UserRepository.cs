using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.DataTransferObjects.User;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Models;

namespace sigma_backend.Repository
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }
        public async Task<IEnumerable<User>> SearchByUsernameAsync(string query)
        {
            return await _context.Users
                .Include(u => u.Profile)
                .Where(u => u.UserName != null && u.UserName.Contains(query))
                .Take(10)
                .ToListAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task SaveExpoToken(string username, string token)
        {
            var existing = await _context.PushTokens.FirstOrDefaultAsync(t => t.Username == username && t.Token == token);
            if (existing == null)
            {
                _context.PushTokens.Add(new PushToken { Username = username, Token = token });
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<string>> GetExpoTokens(string username)
        {
            return await _context.PushTokens
                .Where(t => t.Username == username)
                .Select(t => t.Token)
                .ToListAsync();
        }

    }
}