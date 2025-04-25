using sigma_backend.DataTransferObjects.Activity;
using sigma_backend.Models;

namespace sigma_backend.Interfaces.Repository
{
    public interface IUserActivityRepository : IDataChecksumProvider
    {
        Task<UserActivity> CreateAsync(UserActivity userActivityModel);
        Task<UserActivity?> DeleteAsync(Guid id);
        Task<List<UserActivity>> GetAllByUserIdAsync(string userId);
        Task<UserActivity?> GetByIdAsync(Guid id);
        Task<UserActivity?> UpdateAsync(Guid id, UpdateActivityRequestDto activityDto);
    }
}