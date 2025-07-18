using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FilesStorage.Messages;
using FilesStorage.Services;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

public class FileLocalServiceTests
{
    private readonly Mock<IBus> _mockBus;
    private readonly FileLocalService _service;

    public FileLocalServiceTests()
    {
        _mockBus = new Mock<IBus>();
        _service = new FileLocalService(_mockBus.Object);
    }

    [Fact]
    public async Task UploadFilesAsync_ReturnsFalse_WhenFilesIsNull()
    {
        var result = await _service.UploadFilesAsync(null!);
        Assert.False(result.IsSuccess);
        Assert.Equal("Nenhum arquivo recebido.", result.ErrorMessage);
    }

    [Fact]
    public async Task UploadFilesAsync_ReturnsFalse_WhenFilesIsEmpty()
    {
        var result = await _service.UploadFilesAsync(new List<IFormFile>());
        Assert.False(result.IsSuccess);
        Assert.Equal("Nenhum arquivo recebido.", result.ErrorMessage);
    }

    [Fact]
    public async Task UploadFilesAsync_ReturnsFalse_WhenExtensionNotAllowed()
    {
        var files = new List<IFormFile> { CreateFormFile("arquivo.exe", "image/png") };

        var result = await _service.UploadFilesAsync(files);
        Assert.False(result.IsSuccess);
        Assert.Contains("Extensão não permitida", result.ErrorMessage);
    }

    [Fact]
    public async Task UploadFilesAsync_ReturnsFalse_WhenMimeNotAllowed()
    {
        var files = new List<IFormFile> { CreateFormFile("imagem.png", "application/pdf") };

        var result = await _service.UploadFilesAsync(files);
        Assert.False(result.IsSuccess);
        Assert.Contains("Tipo MIME não permitido", result.ErrorMessage);
    }

    [Fact]
    public async Task UploadFilesAsync_CallsPublish_WhenValid()
    {
        var files = new List<IFormFile> { CreateFormFile("imagem.png", "image/png") };

        _mockBus
            .Setup(b => b.Publish(It.IsAny<UploadLocalFileMessage>(), default))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var result = await _service.UploadFilesAsync(files);

        Assert.True(result.IsSuccess);
        _mockBus.Verify(
            b =>
                b.Publish(
                    It.Is<UploadLocalFileMessage>(m =>
                        m.Filename == "imagem.png"
                        && m.ContentType == "image/png"
                        && m.FileBytes.Length > 0
                    ),
                    default
                ),
            Times.Once
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("../file.png")]
    [InlineData("folder/file.png")]
    public async Task DeleteFileAsync_ReturnsFalse_WhenFileNameIsInvalid(string? fileName)
    {
        var result = await _service.DeleteFileAsync(fileName!);
        Assert.False(result.IsSuccess);
        Assert.Equal("Nome de arquivo inválido.", result.ErrorMessage);
    }

    [Fact]
    public async Task DeleteFileAsync_CallsPublish_WhenValid()
    {
        string fileName = "imagem.png";

        _mockBus
            .Setup(b => b.Publish(It.IsAny<DeleteLocalFileMessage>(), default))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var result = await _service.DeleteFileAsync(fileName);

        Assert.True(result.IsSuccess);
        _mockBus.Verify(
            b => b.Publish(It.Is<DeleteLocalFileMessage>(m => m.Filename == fileName), default),
            Times.Once
        );
    }

    private static IFormFile CreateFormFile(string fileName, string contentType)
    {
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write("arquivo simulado");
        writer.Flush();
        ms.Position = 0;

        return new FormFile(ms, 0, ms.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType,
        };
    }
}
