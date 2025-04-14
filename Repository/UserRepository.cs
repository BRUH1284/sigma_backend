using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.DataTransferObjects.User;
using sigma_backend.Interfaces;
using sigma_backend.Models;

namespace sigma_backend.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<User>> SearchByUsernameAsync(string query)
        {
            return await _context.Users
                .Include(u => u.Profile)
                .Where(u => u.UserName != null && u.UserName.Contains(query))
                .Take(10)
                .ToListAsync();
        }

        public async Task<User?> UpdateAsync(string id, UpdateUserRequestDto updateDto)
        {
            var existingUser = await _context.Users.FindAsync(id);

            if (existingUser == null)
            {
                return null;
            }

            existingUser.FirstName = updateDto.FirstName;
            existingUser.LastName = updateDto.LastName;

            await _context.SaveChangesAsync();

            return existingUser;
        }
    }
}