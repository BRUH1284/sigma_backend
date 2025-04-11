using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sigma_backend.DataTransferObjects;
using sigma_backend.DataTransferObjects.Activity;
using sigma_backend.Interfaces;
using sigma_backend.Mappers;

namespace sigma_backend.Controllers
{
    [Route("api/activity")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityRepository _activityRepo;
        public ActivityController(IActivityRepository activityRepo)
        {
            _activityRepo = activityRepo;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var activities = await _activityRepo.GetAllAsync();
            var activityDto = activities.Select(s => s.ToActivityDto());

            return Ok(activities);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var activity = await _activityRepo.GetByIdAsync(id);

            if (activity == null)
            {
                return NotFound();
            }

            return Ok(activity.ToActivityDto());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateActivityRequestDto activityDto)
        {
            var activityModel = activityDto.ToActivityFromCreateDto();

            await _activityRepo.CreateAsync(activityModel);

            return CreatedAtAction(nameof(GetById), new { id = activityModel.Id }, activityModel);
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateActivityRequestDto updateDto)
        {
            var activityModel = await _activityRepo.UpdateAsync(id, updateDto);

            if (activityModel == null)
            {
                return NotFound();
            }

            return Ok(activityModel.ToActivityDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var activityModel = await _activityRepo.DeleteAsync(id);

            if (activityModel == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}