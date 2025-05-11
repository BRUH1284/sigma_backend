namespace sigma_backend.DataTransferObjects.ActivityRecord
{
    public class CreateActivityRecordRequestDto
    {
        public required Guid Id { get; set; }
        public required float Duration { get; set; }
        public required float Kcal { get; set; }
        public required int Time { get; set; }

        // Additional properties for type-specific data
        public Guid? ActivityId { get; set; }  // For UserActivityRecord (nullable if not used)
        public int? ActivityCode { get; set; }  // For BasicActivityRecord (nullable if not used)
    }
}