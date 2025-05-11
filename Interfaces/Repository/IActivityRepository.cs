using sigma_backend.DataTransferObjects.Activity;
using sigma_backend.Models;

namespace sigma_backend.Interfaces.Repository
{
    public interface IActivityRepository
    {
        Task<List<Activity>> GetAllAsync();
        Task<Activity?> GetByCodeAsync(int code);
        Task<Activity?> CreateAsync(Activity activityModel);
        Task<Activity?> UpdateAsync(int code, UpdateActivityRequestDto activityDto);
        Task<Activity?> DeleteAsync(int code);
    }
}