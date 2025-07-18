using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FilesStorage.Messages;
using FilesStorage.Services;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

public class FileCloudServiceTests
{
    private readonly Mock<IBus> _mockBus;
    private readonly FileCloudService _service;

    public FileCloudServiceTests()
    {
        _mockBus = new Mock<IBus>();
        _service = new FileCloudService(_mockBus.Object);
    }

    [Fact]
    public async Task UploadFilesAsync_ReturnsFalse_WhenFilesIsNull()
    {
        var result = await _service.UploadFilesAsync(null!);
        Assert.False(result.IsSuccess);

        Assert.Equal("Nenhum arquivo recebido.", result.ErrorMessage);
    }

    [Fact]
    public async Task UploadFilesAsync_ReturnFalse_WhenFileIsEmpty()
    {
        var result = await _service.UploadFilesAsync(new List<IFormFile>());

        Assert.False(result.IsSuccess);
        Assert.Equal("Nenhum arquivo recebido.", result.ErrorMessage);
    }

    [Fact]
    public async Task UploadFilesAsync_ReturnFalse_WhenFileExtensionNotAllowed()
    {
        var files = new List<IFormFile> { CreateFormFile("file.txt", "text/plain") };

        var result = await _service.UploadFilesAsync(files);
        Assert.False(result.IsSuccess);
        Assert.Contains("Extensão não permitida", result.ErrorMessage);
    }

    [Fact]
    public async Task UploadFilesAsync_ReturnsFalse_WhenContentTypeNotAllowed()
    {
        var files = new List<IFormFile> { CreateFormFile("image.jpg", "application/pdf") };

        var result = await _service.UploadFilesAsync(files);

        Assert.False(result.IsSuccess);
        Assert.Contains("Tipo MIME não permitido", result.ErrorMessage);
    }

    [Fact]
    public async Task UploadFilesAsync_CallPublish_WhenFileValid()
    {
        var files = new List<IFormFile> { CreateFormFile("image.png", "image/png") };

        _mockBus
            .Setup(b => b.Publish(It.IsAny<UploadFileMessage>(), default))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var result = await _service.UploadFilesAsync(files);

        Assert.True(result.IsSuccess);
        Assert.Null(result.ErrorMessage);

        _mockBus.Verify(
            b =>
                b.Publish(
                    It.Is<UploadFileMessage>(m =>
                        m.Filename == "image.png"
                        && m.ContentType == "image/png"
                        && m.FileBytes.Length > 0
                    ),
                    default
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteFileAsync_ReturnsFalse_WhenPublicIdIsNullOrEmpty()
    {
        var result1 = await _service.DeleteFileAsync(null!, "file.png");
        var result2 = await _service.DeleteFileAsync("", "file.png");

        Assert.False(result1.IsSuccess);
        Assert.Equal("PublicId não fornecido.", result1.ErrorMessage);

        Assert.False(result2.IsSuccess);
        Assert.Equal("PublicId não fornecido.", result2.ErrorMessage);
    }

    [Fact]
    public async Task DeleteFileAsync_CallsBusPublish_WhenValid()
    {
        _mockBus
            .Setup(b => b.Publish(It.IsAny<DeleteFileMessageCloud>(), default))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var result = await _service.DeleteFileAsync("public123", "file.png");

        Assert.True(result.IsSuccess);
        Assert.Null(result.ErrorMessage);

        _mockBus.Verify(
            b =>
                b.Publish(
                    It.Is<DeleteFileMessageCloud>(m =>
                        m.PublicId == "public123" && m.FileName == "file.png"
                    ),
                    default
                ),
            Times.Once
        );
    }

    private static IFormFile CreateFormFile(string fileName, string contentType)
    {
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write("dummy data");
        writer.Flush();
        ms.Position = 0;

        return new FormFile(ms, 0, ms.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType,
        };
    }
}
