using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using sigma_backend.Options;
using sigma_backend.Service;
using Xunit;

namespace sigma_backend.Tests.Unit.Services
{
    public class PathServiceTests
    {
        private PathService service;
        private StorageOptions options;
        private HttpRequest request;
        private const string RootPath = "testroot";
        public PathServiceTests()
        {
            var mockOptions = Microsoft.Extensions.Options.Options.Create(new StorageOptions
            {
                RootPath = RootPath,
                UploadsPath = "uploads",
                DownloadsPath = "downloads",
                ProfilePicturesSubfolder = "profile-pictures",
                PostSubfolder = "posts",
                PostImagesSubfolder = "images"
            });

            options = mockOptions.Value;

            // Mock HttpRequest
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(r => r.Scheme).Returns("https");
            mockRequest.Setup(r => r.Host).Returns(new HostString("localhost"));

            request = mockRequest.Object;

            service = new PathService(mockOptions);
        }
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetUserUploadsDirectoryPath_ReturnString(bool createNewDirectory)
        {
            // Arrange
            var username = Guid.NewGuid().ToString();
            var userUploadPath = Path.Combine(
                options.RootPath,
                options.UploadsPath,
                username);

            // Act
            var result = service.GetUserUploadsDirectoryPath(username, createNewDirectory);

            // Assert
            if (createNewDirectory)
            {
                Assert.Equal(userUploadPath, result);
                Assert.True(Directory.Exists(userUploadPath));

                // Cleanup
                if (Directory.Exists(userUploadPath))
                    Directory.Delete(userUploadPath, true);
            }
            else
                Assert.Null(result);

        }
        [Fact]
        public void GetProfilePictureDirectoryPath_ReturnString()
        {
            // Arrange
            var username = Guid.NewGuid().ToString();
            var userProfilePictureDirectoryPath = Path.Combine(
                options.RootPath,
                options.UploadsPath,
                username,
                options.ProfilePicturesSubfolder);


            // Act
            var result = service.GetProfilePictureDirectoryPath(username);

            // Assert
            Assert.Equal(userProfilePictureDirectoryPath, result);
            Assert.True(Directory.Exists(userProfilePictureDirectoryPath));

            // Cleanup
            if (Directory.Exists(userProfilePictureDirectoryPath))
                Directory.Delete(userProfilePictureDirectoryPath, true);
        }
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetProfilePicturePath_ReturnStringOrNull(bool createFile)
        {
            // Arrange
            var profilePictureName = "test-profile-picture.png";
            var username = Guid.NewGuid().ToString();

            var userProfilePictureDirectoryPath = Path.Combine(
                options.RootPath,
                options.UploadsPath,
                username,
                options.ProfilePicturesSubfolder);
            var fullFilePath = Path.Combine(userProfilePictureDirectoryPath, profilePictureName);

            // Create the directory and file
            Directory.CreateDirectory(userProfilePictureDirectoryPath);
            if (createFile)
                File.WriteAllText(fullFilePath, "fake image content");

            // Act
            var result = service.GetProfilePicturePath(username, profilePictureName);

            // Assert
            if (createFile)
                Assert.Equal(fullFilePath, result);
            else
                Assert.Null(result);

            // Cleanup
            if (Directory.Exists(userProfilePictureDirectoryPath))
                Directory.Delete(userProfilePictureDirectoryPath, true);
        }
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetProfilePictureUrl_ReturnStringOrNull(bool createFile)
        {
            // Arrange
            var profilePictureName = "test-profile-picture.png";
            var username = Guid.NewGuid().ToString();

            var userProfilePictureDirectoryPath = Path.Combine(
                options.RootPath,
                options.UploadsPath,
                username,
                options.ProfilePicturesSubfolder);
            var fullFilePath = Path.Combine(userProfilePictureDirectoryPath, profilePictureName);

            // Create the directory and file
            Directory.CreateDirectory(userProfilePictureDirectoryPath);
            if (createFile)
                File.WriteAllText(fullFilePath, "fake image content");

            // Act
            var result = service.GetProfilePictureUrl(request, username, profilePictureName);

            // Assert
            if (createFile)
            {
                Assert.NotNull(result);
                Assert.StartsWith("https://localhost", result);
                Assert.Contains(profilePictureName, result);
            }
            else
                Assert.Null(result);

            // Cleanup
            if (Directory.Exists(userProfilePictureDirectoryPath))
                Directory.Delete(userProfilePictureDirectoryPath, true);
        }
        [Fact]
        public void GetPostImagesDirectoryPath_ReturnString()
        {
            // Arrange
            var username = Guid.NewGuid().ToString();
            var postId = 123;

            var userPostsImagesDirectoryPath = Path.Combine(
                options.RootPath,
                options.UploadsPath,
                username,
                options.PostSubfolder,
                postId.ToString(),
                options.PostImagesSubfolder);

            // Act
            var result = service.GetPostImagesDirectoryPath(username, postId);

            // Assert
            Assert.Equal(userPostsImagesDirectoryPath, result);
            Assert.True(Directory.Exists(userPostsImagesDirectoryPath));

            // Cleanup
            if (Directory.Exists(userPostsImagesDirectoryPath))
                Directory.Delete(userPostsImagesDirectoryPath, true);
        }
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetPostImagePath_ReturnStringOrNull(bool createFile)
        {
            // Arrange
            var postImageName = "test-post-picture.png";
            var username = Guid.NewGuid().ToString();
            var postId = 123;

            var userPostsImagesDirectoryPath = Path.Combine(
                options.RootPath,
                options.UploadsPath,
                username,
                options.PostSubfolder,
                postId.ToString(),
                options.PostImagesSubfolder);
            var fullFilePath = Path.Combine(userPostsImagesDirectoryPath, postImageName);

            // Create the directory and file
            Directory.CreateDirectory(userPostsImagesDirectoryPath);
            if (createFile)
                File.WriteAllText(fullFilePath, "fake image content");

            // Act
            var result = service.GetPostImageUrl(request, username, postId, postImageName);

            // Assert
            if (createFile)
            {
                Assert.NotNull(result);
                Assert.StartsWith("https://localhost", result);
                Assert.Contains(postImageName, result);
            }
            else
                Assert.Null(result);

            // Cleanup
            if (Directory.Exists(userPostsImagesDirectoryPath))
                Directory.Delete(userPostsImagesDirectoryPath, true);
        }
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetPostImageUrl_ReturnStringOrNull(bool createFile)
        {
            // Arrange
            var postImageName = "test-post-picture.png";
            var username = Guid.NewGuid().ToString();
            var postId = 123;

            var userPostsImagesDirectoryPath = Path.Combine(
                options.RootPath,
                options.UploadsPath,
                username,
                options.PostSubfolder,
                postId.ToString(),
                options.PostImagesSubfolder);
            var fullFilePath = Path.Combine(userPostsImagesDirectoryPath, postImageName);

            // Create the directory and file
            Directory.CreateDirectory(userPostsImagesDirectoryPath);
            if (createFile)
                File.WriteAllText(fullFilePath, "fake image content");

            // Act
            var result = service.GetPostImagePath(username, postId, postImageName);

            // Assert
            if (createFile)
            {
                Assert.Equal(fullFilePath, result);
            }
            else
                Assert.Null(result);

            // Cleanup
            if (Directory.Exists(userPostsImagesDirectoryPath))
                Directory.Delete(userPostsImagesDirectoryPath, true);
        }
        [Theory]
        [InlineData($"{RootPath}/some/path", "some/path")]
        [InlineData($"{RootPath}\\some\\path", "some\\path")]
        [InlineData("some/path", "some/path")]
        [InlineData("\\some\\path", "\\some\\path")]
        public void RemoveRoot_ReturnString(string path, string expected)
        {
            // Act
            var result = service.RemoveRoot(path);

            // Assert
            Assert.Equal(result, expected);
        }
        [Fact]
        public void AddRoot_WithoutRoot_ReturnString()
        {
            //Arrange
            var path = Path.Combine("some", "path");

            // Act
            var result = service.AddRoot(path);

            // Assert
            Assert.Equal(result, Path.Combine(RootPath, path));
        }
        [Fact]
        public void AddRoot_WithRoot_ReturnString()
        {
            //Arrange
            var path = Path.Combine(RootPath, "some", "path");

            // Act
            var result = service.AddRoot(path);

            // Assert
            Assert.Equal(result, path);
        }
        [Fact]
        public void BuildPublicUrl_ReturnString()
        {
            // Arrange
            var path = Path.Combine("some", "path");

            // Act
            var result = service.BuildPublicUrl(request, path);

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith("https://localhost", result);
            Assert.Contains(path, result);
        }
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void PublicUrlToRelativePath_ReturnStringOrNull(bool createFile)
        {
            // Arrange
            var fileName = "test-image.png";

            var testDirectoryPath = Path.Combine(
                options.RootPath,
                "some",
                "path");
            var fullFilePath = Path.Combine(testDirectoryPath, fileName);

            // Create the directory and file
            Directory.CreateDirectory(testDirectoryPath);
            if (createFile)
                File.WriteAllText(fullFilePath, "fake image content");

            // Construct a matching public URL
            var relativePath = Path.GetRelativePath(options.RootPath, fullFilePath);
            var publicUrl = $"https://localhost/{relativePath.Replace("\\", "/")}";

            // Act
            var result = service.PublicUrlToRelativePath(publicUrl);

            // Assert
            if (createFile)
            {
                Assert.Equal(fullFilePath, result);
            }
            else
                Assert.Null(result);

            // Cleanup
            if (Directory.Exists(testDirectoryPath))
                Directory.Delete(testDirectoryPath, true);
        }
    }
}