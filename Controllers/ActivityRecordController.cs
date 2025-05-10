using Microsoft.AspNetCore.Mvc;
using sigma_backend.DataTransferObjects.ActivityRecord;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Interfaces.Service;
using sigma_backend.Mappers;
using sigma_backend.Models;

namespace sigma_backend.Controllers
{
    [Route("api/activity-records")]
    [ApiController]
    public class ActivityRecordController : ControllerBase
    {
        private readonly IActivityRepository _activityRepo;
        private readonly IUserActivityRepository _userActivityRepo;
        private readonly IActivityRecordRepository _activityRecordRepo;
        private readonly ICurrentUserService _currentUserService;
        public ActivityRecordController(
            IActivityRepository activityRepo,
            IUserActivityRepository userActivityRepo,
            ICurrentUserService currentUserService,
            IActivityRecordRepository activityRecordRepository)
        {
            _activityRepo = activityRepo;
            _userActivityRepo = userActivityRepo;
            _activityRecordRepo = activityRecordRepository;
            _currentUserService = currentUserService;
        }

        [HttpGet("last-update-time")]
        public async Task<IActionResult> GetRecordsLastUpdateTime()
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user == null)
                return Unauthorized();

            var lastUpdatedAt = await _activityRecordRepo.GetLastUpdateTimeByUserIdAsync(user.Id);

            if (lastUpdatedAt == null)
                return NoContent();

            return Ok(lastUpdatedAt);
        }
        [HttpGet("get-modified-from-epoch")]
        public async Task<IActionResult> GetModifiedRecordsFromEpoch([FromQuery] long fromEpoch)
        {
            var fromDate = DateTimeOffset.FromUnixTimeSeconds(fromEpoch).UtcDateTime;

            return await GetModifiedRecordsFromDate(fromDate);
        }
        [HttpGet("get-modified-from-date")]
        public async Task<IActionResult> GetModifiedRecordsFromDate([FromQuery] DateTime fromDate)
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user == null)
                return Unauthorized();

            // Fetch the records for the user with LastModified >= fromDate
            var records = await _activityRecordRepo.GetByUserIdModifiedAfterAsync(user.Id, fromDate);

            if (records.Count == 0)
                return NoContent();

            return Ok(records.ToActivityRecordDtos());
        }
        [HttpGet]
        public async Task<IActionResult> GetAllRecords()
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user == null)
                return Unauthorized();

            // Fetch the records for the user
            var records = await _activityRecordRepo.GetByUserIdAsync(user.Id);

            if (records.Count == 0)
                return NoContent();

            return Ok(records.ToActivityRecordDtos());
        }
        [HttpPost]
        public async Task<IActionResult> AddRecords([FromBody] List<CreateActivityRecordRequestDto> recordDtos)
        {
            // TODO: check record exists, check activity exists
            if (recordDtos.Count == 0)
                return BadRequest("At least one record is required.");

            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user == null)
                return Unauthorized();

            // Convert DTOs to entity records
            var records = recordDtos.ToActivityRecords(user.Id, DateTime.UtcNow);

            foreach (var record in records)
            {
                if ((await _activityRecordRepo.GetAsync(record.Id)) != null)
                    return BadRequest("Record with this id (" + record.Id + ") already exists.");

                if (record is BasicActivityRecord basicRecord)
                {
                    if ((await _activityRepo.GetByCodeAsync(basicRecord.ActivityCode)) == null)
                        return BadRequest("Activity with this code(" + basicRecord.ActivityCode + ") does not exists.");
                }
                else if (record is UserActivityRecord userRecord)
                {
                    if ((await _userActivityRepo.GetByIdAsync(userRecord.ActivityId)) == null)
                        return BadRequest("User Activity with this code(" + userRecord.ActivityId + ") does not exists.");
                }
            }

            // Call CreateAsync to add the list of records to the database
            var createdRecords = await _activityRecordRepo.CreateAsync(records);

            // Return the created records as DTOs
            var createdRecordDtos = createdRecords.ToActivityRecordDtos();

            return CreatedAtAction(nameof(AddRecords), new { count = createdRecordDtos.Count }, createdRecordDtos);
        }
        [HttpPut]
        public async Task<IActionResult> ModifyRecord([FromBody] UpdateActivityRecordRequestDto updateDto)
        {
            var record = await _activityRecordRepo.UpdateAsync(updateDto);

            if (record == null)
                return NotFound();

            return Ok(record.ToActivityRecordDto());
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteRecord(Guid id)
        {
            var record = await _activityRecordRepo.DeleteAsync(id);

            if (record == null)
                return NotFound();

            return NoContent();
        }
    }
}