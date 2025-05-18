using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sigma_backend.DataTransferObjects.Activity;
using sigma_backend.Enums;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Interfaces.Service;
using sigma_backend.Mappers;

namespace sigma_backend.Controllers
{
    /// <summary>
    /// Controller for managing system and user-defined physical activities.
    /// </summary>
    [Route("api/activity")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityRepository _activityRepo;
        private readonly IUserActivityRepository _userActivityRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDataVersionRepository _dataVersionRepo;
        public ActivityController(
            IActivityRepository activityRepo,
            IUserActivityRepository userActivityRepo,
            ICurrentUserService currentUserService,
            IDataVersionRepository dataVersionRepo)
        {
            _activityRepo = activityRepo;
            _userActivityRepo = userActivityRepo;
            _currentUserService = currentUserService;
            _dataVersionRepo = dataVersionRepo;
        }

        /// <summary>
        /// Returns the timestamp of the last update to the activity dataset.
        /// </summary>
        [HttpGet("last-update-time")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActivitiesLastUpdateTime()
        {
            var lastUpdatedAt = await _dataVersionRepo.LastUpdatedAt(DataResource.Activities);

            if (lastUpdatedAt == null)
                return NoContent();

            return Ok(lastUpdatedAt);
        }

        /// <summary>
        /// Returns all predefined activities available in the system.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllActivities()
        {
            var activities = await _activityRepo.GetAllAsync();
            var activityDtos = activities.Select(s => s.ToActivityDto());

            return Ok(activityDtos);
        }

        /// <summary>
        /// Returns a single activity by its numeric code.
        /// </summary>
        /// <param name="code">Activity code</param>
        /// <returns>Activity DTO</returns>
        [HttpGet("{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActivityByCode(int code)
        {
            var activity = await _activityRepo.GetByCodeAsync(code);

            if (activity == null)
                return NotFound();

            return Ok(activity.ToActivityDto());
        }

        /// <summary>
        /// Creates a new activity (admin only).
        /// </summary>
        /// <param name="activityDto">Activity data</param>
        /// <returns>Created activity</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateActivity([FromBody] CreateActivityRequestDto activityDto)
        {
            var activityModel = activityDto.ToActivityFromCreateDto();

            var activity = await _activityRepo.CreateAsync(activityModel);

            if (activity == null)
                return BadRequest("Activity with this code already exists.");

            return CreatedAtAction(nameof(GetActivityByCode), new { code = activityModel.Code }, activityModel);
        }
        
        /// <summary>
        /// Updates an existing activity (admin only).
        /// </summary>
        /// <param name="code">Activity code</param>
        /// <param name="updateDto">New activity data</param>
        /// <returns>Updated activity DTO</returns>
        [HttpPut("{code}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateActivity(int code, [FromBody] UpdateActivityRequestDto updateDto)
        {
            var activityModel = await _activityRepo.UpdateAsync(code, updateDto);

            if (activityModel == null)
                return NotFound();

            return Ok(activityModel.ToActivityDto());
        }

        /// <summary>
        /// Deletes an activity by code (admin only).
        /// </summary>
        /// <param name="code">Activity code</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{code}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteActivity(int code)
        {
            var activityModel = await _activityRepo.DeleteAsync(code);

            if (activityModel == null)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Returns a checksum of the user's custom activities.
        /// </summary>
        [HttpGet("my/checksum")]
        public async Task<IActionResult> GetUserActivitiesChecksum()
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user == null)
                return Unauthorized();

            var checksum = await _userActivityRepo.ComputeUserDataChecksum(user.Id);

            return Ok(new { Checksum = checksum });
        }

        /// <summary>
        /// Returns all activities created by the current user.
        /// </summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetAllUserActivities()
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user == null)
                return Unauthorized();

            var userActivities = await _userActivityRepo.GetAllByUserIdAsync(user.Id);
            var userActivityDtos = userActivities.Select(s => s.ToUserActivityDto());

            return Ok(userActivityDtos);
        }

        /// <summary>
        /// Returns a user-created activity by its ID.
        /// </summary>
        /// <param name="id">Activity ID (GUID)</param>
        [HttpGet("my/{id}")]
        public async Task<IActionResult> GetUserActivityById(Guid id)
        {
            var userActivity = await _userActivityRepo.GetByIdAsync(id);

            if (userActivity == null)
                return NotFound();

            return Ok(userActivity.ToUserActivityDto());
        }

        /// <summary>
        /// Creates a new user-defined activity.
        /// </summary>
        /// <param name="userActivityDto">Activity data</param>
        [HttpPost("my")]
        public async Task<IActionResult> CreateUserActivity([FromBody] CreateUserActivityRequestDto userActivityDto)
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user == null)
                return Unauthorized();

            var userActivityModel = userActivityDto.ToUserActivityFromCreateDto(user.Id);

            var userActivity = await _userActivityRepo.CreateAsync(userActivityModel);

            return CreatedAtAction(
                nameof(GetUserActivityById),
                new { id = userActivityModel.Id },
                userActivityModel.ToUserActivityDto());
        }

        /// <summary>
        /// Updates an existing user-defined activity.
        /// </summary>
        /// <param name="id">Activity ID (GUID)</param>
        /// <param name="updateDto">Updated activity data</param>
        [HttpPut("my/{id}")]
        public async Task<IActionResult> UpdateUserActivity(Guid id, [FromBody] UpdateActivityRequestDto updateDto)
        {
            var userActivityModel = await _userActivityRepo.UpdateAsync(id, updateDto);

            if (userActivityModel == null)
                return NotFound();

            return Ok(userActivityModel.ToUserActivityDto());
        }

        /// <summary>
        /// Deletes a user-defined activity by its ID.
        /// </summary>
        /// <param name="id">Activity ID (GUID)</param>
        [HttpDelete("my/{id}")]
        public async Task<IActionResult> DeleteUserActivity(Guid id)
        {
            var userActivityModel = await _userActivityRepo.DeleteAsync(id);

            if (userActivityModel == null)
                return NotFound();

            return NoContent();
        }
    }
}