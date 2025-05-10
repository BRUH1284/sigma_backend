using sigma_backend.DataTransferObjects.ActivityRecord;
using sigma_backend.Enums;
using sigma_backend.Models;

namespace sigma_backend.Mappers
{
    public static class ActivityRecordMapper
    {
        public static ActivityRecordDto ToActivityRecordDto(this ActivityRecord record)
        {
            // Create the base DTO
            var dto = new ActivityRecordDto
            {
                Id = record.Id,
                Duration = record.Duration,
                Kcal = record.Kcal,
                Time = record.Time,
                LastModified = record.LastModified,
            };

            if (record is UserActivityRecord userRecord)
            {
                dto.ActivityId = userRecord.ActivityId;
            }
            else if (record is BasicActivityRecord basicRecord)
            {
                dto.ActivityCode = basicRecord.ActivityCode;
            }

            return dto;
        }

        public static List<ActivityRecordDto> ToActivityRecordDtos(this List<ActivityRecord> records)
        {
            return records.Select(ToActivityRecordDto).ToList();
        }
        public static ActivityRecord ToActivityRecord(this CreateActivityRecordRequestDto dto, string userId, DateTime lastModified)
        {
            ActivityRecord record;

            if (dto.ActivityId.HasValue)
            {
                record = new UserActivityRecord
                {
                    Id = dto.Id,
                    UserId = userId,
                    Duration = dto.Duration,
                    Kcal = dto.Kcal,
                    Time = dto.Time.ToUniversalTime(),
                    LastModified = lastModified.ToUniversalTime(),
                    Type = RecordType.User,
                    ActivityId = dto.ActivityId.Value
                };
            }
            else if (dto.ActivityCode.HasValue)
            {
                record = new BasicActivityRecord
                {
                    Id = dto.Id,
                    UserId = userId,
                    Duration = dto.Duration,
                    Kcal = dto.Kcal,
                    Time = dto.Time.ToUniversalTime(),
                    LastModified = lastModified.ToUniversalTime(),
                    Type = RecordType.Basic,
                    ActivityCode = dto.ActivityCode.Value
                };
            }
            else
            {
                throw new ArgumentException("ActivityRecordDto must have either ActivityId or ActivityCode to determine the type.");
            }

            return record;
        }
        public static List<ActivityRecord> ToActivityRecords(this List<CreateActivityRecordRequestDto> dtos, string userId, DateTime lastModified)
        {
            return dtos.Select(record => record.ToActivityRecord(userId, lastModified)).ToList();
        }
        public static ActivityRecord ToActivityRecord(this ActivityRecordDto dto, string userId)
        {
            ActivityRecord record;

            if (dto.ActivityId.HasValue)
            {
                record = new UserActivityRecord
                {
                    Id = dto.Id,
                    UserId = userId,
                    Duration = dto.Duration,
                    Kcal = dto.Kcal,
                    Time = dto.Time.ToUniversalTime(),
                    LastModified = dto.LastModified.ToUniversalTime(),
                    Type = RecordType.User,
                    ActivityId = dto.ActivityId.Value
                };
            }
            else if (dto.ActivityCode.HasValue)
            {
                record = new BasicActivityRecord
                {
                    Id = dto.Id,
                    UserId = userId,
                    Duration = dto.Duration,
                    Kcal = dto.Kcal,
                    Time = dto.Time.ToUniversalTime(),
                    LastModified = dto.LastModified.ToUniversalTime(),
                    Type = RecordType.Basic,
                    ActivityCode = dto.ActivityCode.Value
                };
            }
            else
            {
                throw new ArgumentException("ActivityRecordDto must have either ActivityId or ActivityCode to determine the type.");
            }

            return record;
        }

        public static List<ActivityRecord> ToActivityRecords(this List<ActivityRecordDto> dtos, string userId)
        {
            return dtos.Select(record => record.ToActivityRecord(userId)).ToList();
        }
    }
}