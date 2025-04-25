namespace sigma_backend.Models
{
    public class Activity
    {
        public int Code { get; set; }
        public required string MajorHeading { get; set; }
        public required float MetValue { get; set; }
        public required string Description { get; set; }
    }
}