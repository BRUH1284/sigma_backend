using sigma_backend.Entities; // Importing the entities
using sigma_backend.DataTransferObjects; // Importing Data Transfer Objects (DTOs) for request and response
using sigma_backend.Repositories; // Importing the repositories for data access

namespace sigma_backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository; // Repository instance for database operations
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository; // Injecting the repository via constructor
        }
        // Retrieves all users, converts them to DTOs, and returns the list
        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync(); // Fetch all users from repository

            // Convert each user entity into a UserResponseDto and return the list
            return users.Select(u => new UserResponseDto
            {
                Id = u.Id//...
            });
        }
        private async Task<User> GetUserById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id); // Fetch user by ID

            // If the user is not found, throw an exception
            if (user == null)
                throw new KeyNotFoundException("User not found");

            return user;
        }
        // Retrieves a user by ID and converts it to a DTO
        public async Task<UserResponseDto> GetUserByIdAsync(int id)
        {
            var user = await GetUserById(id);

            // Convert entity to DTO and return it
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };
        }

        // Adds a new user using a request DTO
        public async Task AddUserAsync(UserRequestDto userDto)
        {
            // Convert DTO to entity
            var user = new User
            {
                Username = userDto.Username//.. no id
            };

            // Add the new user to the database
            await _userRepository.AddAsync(user);
        }

        // Updates an existing user with new data
        public async Task UpdateUserAsync(int id, UserRequestDto userDto)
        {
            var user = await GetUserById(id);

            // Update user fields with new values from DTO
            user.Username = userDto.Username;

            // Save the updated user in the database
            await _userRepository.UpdateAsync(user);
        }

        // Deletes a user by ID
        public async Task DeleteUserAsync(int id)
        {
            await GetUserById(id);

            // Delete the user from the database
            await _userRepository.DeleteAsync(id);
        }
    }
}