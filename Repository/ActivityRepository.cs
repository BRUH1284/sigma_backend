using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.DataTransferObjects.Activity;
using sigma_backend.Enums;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Models;

namespace sigma_backend.Repository
{
    public class ActivityRepository : RepositoryBase, IActivityRepository
    {
        private readonly IDataVersionRepository _dataVersionRepository;
        private const DataResource dataResource = DataResource.Activities;
        public ActivityRepository(
            IDataVersionRepository dataVersionRepository,
            ApplicationDbContext context) : base(context)
        {
            _dataVersionRepository = dataVersionRepository;
        }

        public async Task<Activity?> CreateAsync(Activity activityModel)
        {
            // Check if activity with this code already exists
            if ((await _context.Activities.FindAsync(activityModel.Code)) != null)
                return null;

            await _context.Activities.AddAsync(activityModel);

            await _context.SaveChangesAsync();

            // Update data version
            await _dataVersionRepository.CreateOrUpdate(dataResource);
            return activityModel;
        }

        public async Task<Activity?> DeleteAsync(int code)
        {
            var activityModel = await _context.Activities.FindAsync(code);

            if (activityModel == null)
                return null;

            _context.Activities.Remove(activityModel);

            await _context.SaveChangesAsync();

            // Update data version
            await _dataVersionRepository.CreateOrUpdate(dataResource);
            return activityModel;
        }

        public async Task<List<Activity>> GetAllAsync()
        {
            return await _context.Activities.ToListAsync();
        }

        public async Task<Activity?> GetByCodeAsync(int code)
        {
            return await _context.Activities.FindAsync(code);
        }

        public async Task<Activity?> UpdateAsync(int code, UpdateActivityRequestDto activityDto)
        {
            var existingActivity = await _context.Activities.FindAsync(code);

            if (existingActivity == null)
                return null;

            existingActivity.MajorHeading = activityDto.MajorHeading;
            existingActivity.MetValue = activityDto.MetValue;
            existingActivity.Description = activityDto.Description;

            await _context.SaveChangesAsync();

            // Update data version
            await _dataVersionRepository.CreateOrUpdate(dataResource);
            return existingActivity;
        }
    }
}