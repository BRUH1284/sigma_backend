namespace sigma_backend.Interfaces.Service
{
    public interface IPathService
    {
        string GetProfilePictureFolderPath(string username);
        string? GetProfilePicturePath(string username, string fileName);
        string? GetProfilePictureUrl(HttpRequest httpRequest, string username, string fileName);
        string GetPostImagesFolderPath(string username, int postId);
        string? GetPostImagePath(string username, int postId, string fileName);
        string? GetPostImageUrl(HttpRequest httpRequest, string username, int postId, string fileName);
        string BuildPublicUrl(HttpRequest request, string relativePath);
        string RemoveRoot(string path);
        string AddRoot(string path);
        string? PublicUrlToRelativePath(string url);
    }
}