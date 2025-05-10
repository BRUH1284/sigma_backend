namespace sigma_backend.DataTransferObjects.ActivityRecord
{
    public class ActivityRecordDto
    {
        public required Guid Id { get; set; }
        public required float Duration { get; set; }
        public required float Kcal { get; set; }
        public required DateTime Time { get; set; }
        public required DateTime LastModified { get; set; }

        // Additional properties for type-specific data
        public Guid? ActivityId { get; set; }  // For UserActivityRecord (nullable if not used)
        public int? ActivityCode { get; set; }  // For BasicActivityRecord (nullable if not used)
    }
}