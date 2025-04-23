using System.Text;
using Microsoft.AspNetCore.Http;
using Moq;
using sigma_backend.Service;
using Xunit;

namespace sigma_backend.Tests.Unit.Services
{
    public class FileServiceTests
    {
        private readonly FileService service;

        public FileServiceTests()
        {
            service = new FileService();
        }
        [Theory]
        [InlineData("image/jpeg", true)]
        [InlineData("image/png", true)]
        [InlineData("application/pdf", false)]
        public void IsImage_ReturnsExpected(string contentType, bool expected)
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.ContentType).Returns(contentType);

            // Act
            var result = service.IsImage(mockFile.Object);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task SaveImageAsync_SavesFileAndReturnsName()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            var content = "fake image content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.png");
            mockFile.Setup(f => f.ContentType).Returns("image/png");
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                    .Returns<Stream, CancellationToken>((s, _) => stream.CopyToAsync(s));

            try
            {
                // Act
                var fileName = await service.SaveImageAsync(mockFile.Object, tempDir);
                var filePath = Path.Combine(tempDir, fileName);

                // Assert
                Assert.True(File.Exists(filePath));
            }
            finally
            {
                // Cleanup
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void GetContentType_ReturnsExpectedTypes()
        {
            // Act + Assert
            Assert.Equal("image/jpeg", service.GetContentType("photo.jpg")); // known type
            Assert.Equal("application/octet-stream", service.GetContentType("unknownfile.xyz")); // fallback
        }
        [Fact]
        public void DeleteFile_RemovesExistingFile()
        {
            // Arrange: create a temp file
            var tempFile = Path.GetTempFileName();

            // Act
            service.DeleteFile(tempFile);

            // Assert
            Assert.False(File.Exists(tempFile));
        }
        [Fact]
        public void DeleteDirectory_RemovesExistingDirectory()
        {
            // Arrange: create a temp directory with a file in it
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(Path.Combine(tempDir, "dummy.txt"), "data");

            // Act
            service.DeleteDirectory(tempDir);

            // Assert
            Assert.False(Directory.Exists(tempDir));
        }
        [Fact]
        public void GetFileStream_ReturnsReadableStream()
        {
            // Arrange: create a temp file with some content
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "hello test");

            try
            {
                // Act
                using var stream = service.GetFileStream(tempFile);
                using var reader = new StreamReader(stream);
                var content = reader.ReadToEnd();

                // Assert
                Assert.Equal("hello test", content);
            }
            finally
            {
                // Cleanup
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
        [Fact]
        public void IsExist_ReturnTrue()
        {
            // Arrange: create a temp directory with a file in it
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var filePath = Path.Combine(tempDir, "dummy.txt");
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(filePath, "data");

            // Act
            var result = service.IsExist(filePath);

            // Assert
            Assert.True(result);

            // Cleanup
            if (File.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
        [Theory]
        [InlineData("some/file.txt")]
        [InlineData("")]
        public void IsExist_ReturnFalse(string filePath)
        {
            // Act
            var result = service.IsExist(filePath);

            // Assert
            Assert.False(result);
        }
    }
}
