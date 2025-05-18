using Microsoft.AspNetCore.Mvc;
using sigma_backend.DataTransferObjects.ActivityRecord;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Interfaces.Service;
using sigma_backend.Mappers;
using sigma_backend.Models;

namespace sigma_backend.Controllers
{

    /// <summary>
    /// Controller for managing activity records for authenticated users.
    /// </summary>
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

        /// <summary>
        /// Gets the last update time for the user's activity records.
        /// </summary>
        /// <returns>Datetime of the last update.</returns>
        /// <response code="200">Returns the last updated datetime</response>
        /// <response code="204">No records exist</response>
        /// <response code="401">Unauthorized</response>
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

        /// <summary>
        /// Gets records modified after the provided UNIX timestamp.
        /// </summary>
        /// <param name="fromEpoch">UNIX timestamp in seconds</param>
        /// <returns>List of modified activity records</returns>
        /// <response code="200">Returns modified records</response>
        /// <response code="204">No records found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("get-modified-from-epoch")]
        public async Task<IActionResult> GetModifiedRecordsFromEpoch([FromQuery] long fromEpoch)
        {
            var fromDate = DateTimeOffset.FromUnixTimeSeconds(fromEpoch).UtcDateTime;

            return await GetModifiedRecordsFromDate(fromDate);
        }

        /// <summary>
        /// Gets records modified after the specified date.
        /// </summary>
        /// <param name="fromDate">UTC datetime</param>
        /// <returns>List of modified activity records</returns>
        /// <response code="200">Returns modified records</response>
        /// <response code="204">No records found</response>
        /// <response code="401">Unauthorized</response>
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

        /// <summary>
        /// Gets all activity records for the current user.
        /// </summary>
        /// <returns>List of all activity records</returns>
        /// <response code="200">Returns activity records</response>
        /// <response code="204">No records exist</response>
        /// <response code="401">Unauthorized</response>
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

        /// <summary>
        /// Adds new activity records for the current user.
        /// </summary>
        /// <param name="recordDtos">List of records to add</param>
        /// <returns>List of created records</returns>
        /// <response code="201">Records successfully created</response>
        /// <response code="400">Invalid or duplicate records</response>
        /// <response code="401">Unauthorized</response>
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

        /// <summary>
        /// Updates an existing activity record.
        /// </summary>
        /// <param name="updateDto">Record update data</param>
        /// <returns>The updated record</returns>
        /// <response code="200">Record updated</response>
        /// <response code="404">Record not found</response>
        [HttpPut]
        public async Task<IActionResult> ModifyRecord([FromBody] UpdateActivityRecordRequestDto updateDto)
        {
            var record = await _activityRecordRepo.UpdateAsync(updateDto);

            if (record == null)
                return NotFound();

            return Ok(record.ToActivityRecordDto());
        }

        /// <summary>
        /// Deletes an activity record by ID.
        /// </summary>
        /// <param name="id">Record ID</param>
        /// <response code="204">Record deleted</response>
        /// <response code="404">Record not found</response>
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