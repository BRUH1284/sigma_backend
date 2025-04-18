using System;

namespace sigma_backend.Models
{
    public class MealFile
    {
        public int Id { get; set; }
        public int MealId { get; set; }
        public string FilePath { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public DateTime UploadedAt { get; set; }

        public virtual Meal Meal { get; set; } = null!;
    }
}
