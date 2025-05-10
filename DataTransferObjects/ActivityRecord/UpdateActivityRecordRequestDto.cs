namespace sigma_backend.DataTransferObjects.ActivityRecord
{
    public class UpdateActivityRecordRequestDto
    {
        public required Guid Id { get; set; }
        public required float Duration { get; set; }
        public required float Kcal { get; set; }
        public required DateTime Time { get; set; }
    }
}