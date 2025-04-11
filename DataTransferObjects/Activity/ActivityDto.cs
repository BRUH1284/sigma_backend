namespace sigma_backend.DataTransferObjects
{
    public class ActivityDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required int KcalPerHour { get; set; }
    }
}