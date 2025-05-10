using sigma_backend.DataTransferObjects.ActivityRecord;
using sigma_backend.Models;

namespace sigma_backend.Interfaces.Repository
{
    public interface IActivityRecordRepository
    {
        Task<List<ActivityRecord>> CreateAsync(List<ActivityRecord> records);
        Task<List<ActivityRecord>> GetByUserIdModifiedAfterAsync(string userId, DateTime modifiedAfter);
        Task<List<ActivityRecord>> GetAllAsync();
        Task<ActivityRecord?> GetAsync(Guid id);
        Task<List<ActivityRecord>> GetByUserIdAsync(string userId);
        Task<ActivityRecord?> DeleteAsync(Guid id);
        Task<ActivityRecord?> UpdateAsync(UpdateActivityRecordRequestDto updateDto);
        Task<DateTime?> GetLastUpdateTimeByUserIdAsync(string userId);
    }
}