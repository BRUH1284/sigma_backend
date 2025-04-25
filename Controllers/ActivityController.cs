using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sigma_backend.DataTransferObjects.Activity;
using sigma_backend.Enums;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Interfaces.Service;
using sigma_backend.Mappers;

namespace sigma_backend.Controllers
{
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
        [HttpGet("last-update-time")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActivitiesLastUpdateTime()
        {
            var lastUpdatedAt = await _dataVersionRepo.LastUpdatedAt(DataResource.Activities);

            if (lastUpdatedAt == null)
                return NoContent();

            return Ok(lastUpdatedAt);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllActivities()
        {
            var activities = await _activityRepo.GetAllAsync();
            var activityDtos = activities.Select(s => s.ToActivityDto());

            return Ok(activityDtos);
        }

        [HttpGet("{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActivityByCode(int code)
        {
            var activity = await _activityRepo.GetByCodeAsync(code);

            if (activity == null)
                return NotFound();

            return Ok(activity.ToActivityDto());
        }

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

        [HttpPut("{code}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateActivity(int code, [FromBody] UpdateActivityRequestDto updateDto)
        {
            var activityModel = await _activityRepo.UpdateAsync(code, updateDto);

            if (activityModel == null)
                return NotFound();

            return Ok(activityModel.ToActivityDto());
        }

        [HttpDelete("{code}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteActivity(int code)
        {
            var activityModel = await _activityRepo.DeleteAsync(code);

            if (activityModel == null)
                return NotFound();

            return NoContent();
        }
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
        [HttpGet("my/{id}")]
        public async Task<IActionResult> GetUserActivityById(Guid id)
        {
            var userActivity = await _userActivityRepo.GetByIdAsync(id);

            if (userActivity == null)
                return NotFound();

            return Ok(userActivity.ToUserActivityDto());
        }
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
        [HttpPut("my/{id}")]
        public async Task<IActionResult> UpdateUserActivity(Guid id, [FromBody] UpdateActivityRequestDto updateDto)
        {
            var userActivityModel = await _userActivityRepo.UpdateAsync(id, updateDto);

            if (userActivityModel == null)
                return NotFound();

            return Ok(userActivityModel.ToUserActivityDto());
        }
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