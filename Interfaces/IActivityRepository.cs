using sigma_backend.DataTransferObjects.Activity;
using sigma_backend.Models;

namespace sigma_backend.Interfaces
{
    public interface IActivityRepository
    {
        Task<List<Activity>> GetAllAsync();

        Task<Activity?> GetByIdAsync(int id);
        Task<Activity> CreateAsync(Activity activityModel);
        Task<Activity?> UpdateAsync(int id, UpdateActivityRequestDto activityDto);
        Task<Activity?> DeleteAsync(int id);
    }
}