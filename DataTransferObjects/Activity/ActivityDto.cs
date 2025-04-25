namespace sigma_backend.DataTransferObjects.Activity
{
    public class ActivityDto
    {
        public required int Code { get; set; }
        public required string MajorHeading { get; set; }
        public required float MetValue { get; set; }
        public required string Description { get; set; }
    }
}