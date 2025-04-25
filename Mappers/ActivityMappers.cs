using sigma_backend.DataTransferObjects.Activity;
using sigma_backend.Models;

namespace sigma_backend.Mappers
{
    public static class ActivityMappers
    {
        public static ActivityDto ToActivityDto(this Activity activityModel)
        {
            return new ActivityDto
            {
                Code = activityModel.Code,
                MajorHeading = activityModel.MajorHeading,
                MetValue = activityModel.MetValue,
                Description = activityModel.Description
            };
        }
        public static UserActivityDto ToUserActivityDto(this UserActivity userActivityModel)
        {
            return new UserActivityDto
            {
                Id = userActivityModel.Id,
                MajorHeading = userActivityModel.MajorHeading,
                MetValue = userActivityModel.MetValue,
                Description = userActivityModel.Description
            };
        }
        public static Activity ToActivityFromCreateDto(this CreateActivityRequestDto activityDto)
        {
            return new Activity
            {
                Code = activityDto.Code,
                MajorHeading = activityDto.MajorHeading,
                MetValue = activityDto.MetValue,
                Description = activityDto.Description
            };
        }
        public static UserActivity ToUserActivityFromCreateDto(
            this CreateUserActivityRequestDto userActivityDto,
            string userId)
        {
            return new UserActivity
            {
                UserId = userId,
                MajorHeading = userActivityDto.MajorHeading,
                MetValue = userActivityDto.MetValue,
                Description = userActivityDto.Description
            };
        }
    }
}
