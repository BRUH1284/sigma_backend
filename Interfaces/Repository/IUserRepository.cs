using sigma_backend.DataTransferObjects.User;
using sigma_backend.Models;

namespace sigma_backend.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> SearchByUsernameAsync(string query);
        Task<User?> UpdateAsync(string id, UpdateUserRequestDto updateDto);
    }
}