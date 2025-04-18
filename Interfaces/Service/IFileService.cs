namespace sigma_backend.Interfaces.Service
{
    public interface IFileService
    {
        bool IsImage(IFormFile file);
        Task<string> SaveImageAsync(IFormFile file, string savePath);
        Stream GetFileStream(string path);
        string GetContentType(string fileName);
        void DeleteFile(string path);
    }
}