namespace sigma_backend.Models
{
    public class BasicActivityRecord : ActivityRecord
    {
        public required int ActivityCode { get; set; }
        public virtual Activity? Activity { get; set; }
    }
}