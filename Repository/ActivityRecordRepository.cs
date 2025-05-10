using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.DataTransferObjects.ActivityRecord;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Models;

namespace sigma_backend.Repository
{
    public class ActivityRecordRepository : RepositoryBase, IActivityRecordRepository
    {
        public ActivityRecordRepository(ApplicationDbContext context) : base(context) { }
        public async Task<List<ActivityRecord>> CreateAsync(List<ActivityRecord> records)
        {
            // Add multiple records at once
            await _context.ActivityRecords.AddRangeAsync(records);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Return the list of created records
            return records;
        }
        public async Task<List<ActivityRecord>> GetAllAsync()
        {
            return await _context.ActivityRecords.ToListAsync();
        }

        public async Task<ActivityRecord?> GetAsync(Guid id)
        {
            return await _context.ActivityRecords.FindAsync(id);
        }
        public async Task<List<ActivityRecord>> GetByUserIdModifiedAfterAsync(string userId, DateTime modifiedAfter)
        {
            return await _context.ActivityRecords
                .Where(r => r.UserId == userId && r.LastModified > modifiedAfter)
                .ToListAsync();
        }
        public async Task<List<ActivityRecord>> GetByUserIdAsync(string userId)
        {
            return await _context.ActivityRecords
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }
        public async Task<ActivityRecord?> DeleteAsync(Guid id)
        {
            var record = await _context.ActivityRecords.FindAsync(id);

            if (record == null)
                return null;

            _context.ActivityRecords.Remove(record);

            await _context.SaveChangesAsync();
            return record;
        }
        public async Task<ActivityRecord?> UpdateAsync(UpdateActivityRecordRequestDto updateDto)
        {
            // Find the existing record
            var existingRecord = await _context.ActivityRecords.FindAsync(updateDto.Id);

            if (existingRecord == null)
                return null;

            // Update properties of the existing record
            existingRecord.Duration = updateDto.Duration;
            existingRecord.Kcal = updateDto.Kcal;
            existingRecord.Time = updateDto.Time.ToUniversalTime();
            existingRecord.LastModified = DateTime.UtcNow; // Update LastModified time

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return existingRecord;
        }
        public async Task<DateTime?> GetLastUpdateTimeByUserIdAsync(string userId)
        {
            var lastUpdateTime = await _context.ActivityRecords
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.LastModified)
                .Select(r => r.LastModified)
                .FirstOrDefaultAsync();

            return lastUpdateTime;
        }
    }
}