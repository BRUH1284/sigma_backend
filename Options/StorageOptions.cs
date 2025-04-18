namespace sigma_backend.Options
{
    public class StorageOptions
    {
        public required string RootPath { get; set; }
        public required string UploadsPath { get; set; }
        public required string ProfilePicturesSubfolder { get; set; }
        public required string PostSubfolder { get; set; }
        public required string PostImagesSubfolder { get; set; }
    }
}