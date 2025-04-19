using Microsoft.Extensions.Options;
using sigma_backend.Interfaces.Service;
using sigma_backend.Options;

public class PathService : IPathService
{
    private readonly StorageOptions _options;

    public PathService(IOptions<StorageOptions> options)
    {
        _options = options.Value;
    }
    public string? GetUserUploadsDirectoryPath(string username, bool create = true)
    {
        // Generate path
        var path = Path.Combine(
            _options.RootPath,
            _options.UploadsPath,
            username);

        // Create directory if it not exists
        if (!Directory.Exists(path) && create)
            Directory.CreateDirectory(path);

        return Directory.Exists(path) ? path : null;
    }
    public string GetProfilePictureFolderPath(string username)
    {
        // Generate path
        var path = Path.Combine(
            GetUserUploadsDirectoryPath(username)!,
            _options.ProfilePicturesSubfolder
        );

        // Create directory if it not exists
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }
    public string? GetProfilePicturePath(string username, string fileName)
    {
        var profilePictureFolderPath = GetProfilePictureFolderPath(username);

        var path = Path.Combine(
            profilePictureFolderPath,
            fileName
        );

        // Return path if it exists
        return File.Exists(path) ? path : null;
    }
    public string? GetProfilePictureUrl(HttpRequest httpRequest, string username, string fileName)
    {
        var path = GetProfilePicturePath(username, fileName);
        // If path exists return public url
        return path == null ? null : BuildPublicUrl(httpRequest, path);
    }
    public string GetPostImagesFolderPath(string username, int postId)
    {
        // Generate Post
        var path = Path.Combine(
            GetUserUploadsDirectoryPath(username)!,
            _options.PostSubfolder,
            postId.ToString(),
            _options.PostImagesSubfolder
        );

        // Create directory if it not exists
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }
    public string? GetPostImagePath(string username, int postId, string fileName)
    {
        var postImagesFolderPath = GetPostImagesFolderPath(username, postId);

        var path = Path.Combine(
            postImagesFolderPath,
            fileName
        );

        // Return path if it exists
        return File.Exists(path) ? path : null;
    }
    public string? GetPostImageUrl(HttpRequest httpRequest, string username, int postId, string fileName)
    {
        var path = GetPostImagePath(username, postId, fileName);
        // If path exists return public url
        return path == null ? null : BuildPublicUrl(httpRequest, path);
    }
    public string RemoveRoot(string path)
    {
        if (path.StartsWith(_options.RootPath))
            return path.Substring(_options.RootPath.Length + 1);

        return path;
    }
    public string AddRoot(string path)
    {
        if (!path.StartsWith(_options.RootPath))
            return Path.Combine(_options.RootPath, path);

        return path;
    }
    public string BuildPublicUrl(HttpRequest request, string relativePath)
    {
        // Normalize path and remove leading "wwwroot/" if present
        var cleanedPath = relativePath.Replace("\\", "/");
        if (cleanedPath.StartsWith(_options.RootPath))
            cleanedPath = cleanedPath.Substring($"{_options.RootPath}/".Length);
        // Return url
        return $"{request.Scheme}://{request.Host}/{cleanedPath}";
    }
    public string? PublicUrlToRelativePath(string url)
    {
        // Generate path
        var path = Path.Combine(_options.RootPath, Path.Combine(new Uri(url).LocalPath.Split('/')));
        // Return path if it exists
        return File.Exists(path) ? path : null;
    }

}