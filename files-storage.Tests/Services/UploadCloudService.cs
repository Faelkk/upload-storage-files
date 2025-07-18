using FilesStorage.Services;
using Moq;
using Xunit;

public class UploadCloudServiceTests
{
    [Fact]
    public async Task UploadFileAsync_ReturnsUrl_OnSuccess()
    {
        var mockService = new Mock<IUploadCloudService>();
        var fileName = "test.png";
        var fileStream = new MemoryStream(new byte[] { 1, 2, 3 });

        mockService
            .Setup(s => s.UploadFileAsync(fileName, fileStream))
            .ReturnsAsync("https://cloudinary.com/test.png");

        var result = await mockService.Object.UploadFileAsync(fileName, fileStream);

        // Assert
        Assert.Equal("https://cloudinary.com/test.png", result);
    }

    [Fact]
    public async Task DeleteFileAsync_ReturnsTrue_OnSuccess()
    {
        // Arrange
        var mockService = new Mock<IUploadCloudService>();
        var publicId = "abc123";

        mockService.Setup(s => s.DeleteFileAsync(publicId)).ReturnsAsync(true);

        var result = await mockService.Object.DeleteFileAsync(publicId);

        Assert.True(result);
    }
}
