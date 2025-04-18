using sigma_backend.Interfaces.Service;
using sigma_backend.Models;

namespace sigma_backend.Extensions
{
    public static class UserExtensions
    {
        public static string? GetProfilePictureUrl(this User user, HttpRequest request, IPathService pathService)
        {
            if (user?.UserName == null || user?.Profile?.ProfilePictureFileName == null)
                return null;

            return pathService.GetProfilePictureUrl(
                request,
                user.UserName,
                user.Profile.ProfilePictureFileName
                );
        }
    }
}