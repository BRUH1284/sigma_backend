using Microsoft.EntityFrameworkCore; // Importing Entity Framework Core for database operations
using sigma_backend.Data; // Importing the application database context
using sigma_backend.Entities; // Importing the User entity

namespace sigma_backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context; // Database context for interacting with the database
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        // Retrieves all users from the database
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            // Converts the Users table into a list and returns it asynchronously
            return await _context.Users.ToListAsync();
        }
        // Retrieves a user by its ID
        public async Task<User> GetByIdAsync(int id)
        {
            // Uses FindAsync to search for a user by id
            return await _context.Users.FindAsync(id); // check for null?
        }
        // Adds a new user to the database
        public async Task AddAsync(User user)
        {
            // Adds user entity to the database context
            await _context.Users.AddAsync(user);

            // Saves the changes to the database asynchronously
            await _context.SaveChangesAsync();
        }
        // Updates an existing user in the database
        public async Task UpdateAsync(User user)
        {
            // Marks user entity as updated in the database context
            _context.Users.Update(user);

            // Saves updated user data to the database asynchronously
            await _context.SaveChangesAsync();
        }
        // Deletes a user by its ID
        public async Task DeleteAsync(int id)
        {
            // Finds user in the database by id
            var user = await _context.Users.FindAsync(id);

            // If user exists, remove it from the database
            if (user != null)
            {
                _context.Users.Remove(user);

                // Saves the changes to the database asynchronously
                await _context.SaveChangesAsync();
            }
        }
    }
}