namespace sigma_backend.DataTransferObjects.Activity
{
    public class UserActivityDto
    {
        public required Guid Id { get; set; }
        public required string MajorHeading { get; set; }
        public required float MetValue { get; set; }
        public required string Description { get; set; }
    }
}