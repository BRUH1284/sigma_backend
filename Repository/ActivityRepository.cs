using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.DataTransferObjects.Activity;
using sigma_backend.Interfaces;
using sigma_backend.Models;

namespace sigma_backend.Repository
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly ApplicationDbContext _context;
        public ActivityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Activity> CreateAsync(Activity activityModel)
        {
            await _context.Activities.AddAsync(activityModel);
            await _context.SaveChangesAsync();
            return activityModel;
        }

        public async Task<Activity?> DeleteAsync(int id)
        {
            var activityModel = await _context.Activities.FindAsync(id);

            if (activityModel == null)
            {
                return null;
            }

            _context.Activities.Remove(activityModel);
            await _context.SaveChangesAsync();
            return activityModel;
        }

        public async Task<List<Activity>> GetAllAsync()
        {
            return await _context.Activities.ToListAsync();
        }

        public async Task<Activity?> GetByIdAsync(int id)
        {
            return await _context.Activities.FindAsync(id);
        }

        public async Task<Activity?> UpdateAsync(int id, UpdateActivityRequestDto activityDto)
        {
            var existingActivity = await _context.Activities.FindAsync(id);

            if (existingActivity == null)
            {
                return null;
            }

            existingActivity.Name = activityDto.Name;
            existingActivity.KcalPerHour = activityDto.KcalPerHour;

            await _context.SaveChangesAsync();

            return existingActivity;
        }
    }
}