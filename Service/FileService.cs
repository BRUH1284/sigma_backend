using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using sigma_backend.Interfaces.Service;

namespace sigma_backend.Service
{
    public class FileService : IFileService
    {
        // Check is file is image
        public bool IsImage(IFormFile file)
        {
            return file.ContentType.StartsWith("image/");
        }

        public bool IsExist(string path)
        {
            if (path.IsNullOrEmpty())
                return false;
            return File.Exists(path);
        }
        public async Task<string> SaveImageAsync(IFormFile file, string savePath)
        {
            // Check if file is an image
            if (!IsImage(file))
                throw new InvalidOperationException("The uploaded file is not a valid image.");

            Directory.CreateDirectory(savePath);

            // Construct full path
            var fileExt = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExt}";
            var fullPath = Path.Combine(savePath, fileName);

            // Save images
            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }
        public void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        public Stream GetFileStream(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }

        public string GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            return provider.TryGetContentType(fileName, out var contentType)
                ? contentType
                : "application/octet-stream"; // fallback to binary
        }
    }
}