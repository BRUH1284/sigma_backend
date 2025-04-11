using sigma_backend.DataTransferObjects;
using sigma_backend.Models;

namespace sigma_backend.Mappers
{
    public static class ActivityMappers
    {
        public static ActivityDto ToActivityDto(this Activity activityModel)
        {
            return new ActivityDto
            {
                Id = activityModel.Id,
                Name = activityModel.Name,
                KcalPerHour = activityModel.KcalPerHour
            };
        }
        public static Activity ToActivityFromCreateDto(this CreateActivityRequestDto activityDto)
        {
            return new Activity
            {
                Name = activityDto.Name,
                KcalPerHour = activityDto.KcalPerHour
            };
        }
    }
}
