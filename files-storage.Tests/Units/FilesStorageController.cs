namespace FileStorage.Tests.Controllers;

using FilesStorage.Controllers;
using FilesStorage.Dtos;
using FilesStorage.Models;
using FilesStorage.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class FilesStorageControllerUnitsTests
{
    private readonly Mock<IFileLocalService> _mockFileLocalService;
    private readonly Mock<IFileCloudService> _mockFileCloudService;
    private readonly Mock<ISaveStorageService> _mockSaveStorageService;

    private readonly FilestorageController _controller;

    public FilesStorageControllerUnitsTests()
    {
        _mockFileCloudService = new Mock<IFileCloudService>();
        _mockFileLocalService = new Mock<IFileLocalService>();
        _mockSaveStorageService = new Mock<ISaveStorageService>();

        _controller = new FilestorageController(
            _mockFileLocalService.Object,
            _mockFileCloudService.Object,
            _mockSaveStorageService.Object
        );
    }

    [Fact]
    public async Task UploadFilesCloud_ReturnsOk_OnSuccess()
    {
        var files = new List<IFormFile>();

        _mockFileCloudService.Setup(s => s.UploadFilesAsync(files)).ReturnsAsync((true, null));

        var result = await _controller.UploadFilesCloud(files);

        var okResult = Assert.IsType<OkObjectResult>(result);

        var response = Assert.IsType<UploadResponseDto>(okResult.Value);

        Assert.Equal(files.Count, response.Count);
        Assert.Equal("Upload na nuvem em processamento", response.Message);
    }

    [Fact]
    public async Task UploadFilesCloud_ReturnsBadRequest_OnFailure()
    {
        var files = new List<IFormFile>();
        _mockFileCloudService
            .Setup(s => s.UploadFilesAsync(files))
            .ReturnsAsync((false, "Erro no upload"));

        var result = await _controller.UploadFilesCloud(files);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        Assert.NotNull(badRequestResult.Value);
        Assert.Equal("Erro no upload", badRequestResult.Value);
    }

    [Fact]
    public async Task UploadFilesLocal_ReturnsBadRequest_OnFailure()
    {
        var files = new List<IFormFile>();
        _mockFileLocalService
            .Setup(s => s.UploadFilesAsync(files))
            .ReturnsAsync((false, "Erro no upload"));

        var result = await _controller.UploadFilesLocal(files);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        Assert.NotNull(badRequestResult.Value);
        Assert.Equal("Erro no upload", badRequestResult.Value);
    }

    [Fact]
    public async Task DeleteImageLocal_ReturnsOk_OnSucess()
    {
        string fileName = "teste.png";

        _mockFileLocalService.Setup(s => s.DeleteFileAsync(fileName)).ReturnsAsync((true, ""));

        var result = await _controller.DeleteImageLocal(fileName);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<DeleteResponseDto>(okResult.Value);

        Assert.Contains(fileName, response.Message);
    }

    [Fact]
    public async Task DeleteImageCloud_ReturnsOk_OnSucess()
    {
        string publicId = "testeawfwas";
        string fileName = "teste.png";

        _mockFileCloudService
            .Setup(s => s.DeleteFileAsync(publicId, fileName))
            .ReturnsAsync((true, ""));

        var result = await _controller.DeleteImageCloud(publicId, fileName);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<DeleteResponseDto>(okResult.Value);

        Assert.Contains(publicId, response.Message);
    }

    [Fact]
    public async Task DeleteImageLocal_ReturnsBadRequest_OnFailure()
    {
        string fileName = "teste.png";

        _mockFileLocalService
            .Setup(s => s.DeleteFileAsync(fileName))
            .ReturnsAsync((false, "Erro na remoção"));

        var result = await _controller.DeleteImageLocal(fileName);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        Assert.NotNull(badRequestResult.Value);
        Assert.Equal("Erro na remoção", badRequestResult.Value);
    }

    [Fact]
    public async Task DeleteImageClod_ReturnsBadRequest_OnFailure()
    {
        string publicId = "Teste";
        string fileName = "teste.png";

        _mockFileCloudService
            .Setup(s => s.DeleteFileAsync(publicId, fileName))
            .ReturnsAsync((false, "Erro na remoção"));

        var result = await _controller.DeleteImageCloud(publicId, fileName);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        Assert.NotNull(badRequestResult.Value);
        Assert.Equal("Erro na remoção", badRequestResult.Value);
    }

    [Fact]
    public async Task GetAllFiles_ReturnsOk_WithFiles()
    {
        var mockFiles = new List<FileRecord>
        {
            new FileRecord { FileName = "file1", ContentType = "text/plain" },
            new FileRecord { FileName = "file2", ContentType = "image/png" },
        };

        _mockSaveStorageService.Setup(s => s.GetAllAsync()).ReturnsAsync(mockFiles);

        var result = await _controller.GetAllFiles();
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockFiles, okResult.Value);
    }

    [Fact]
    public async Task GetFileByName_ReturnsOk_WhenFileExists()
    {
        string fileName = "file1.txt";
        var fileObj = new FileRecord { FileName = fileName, ContentType = "text/plain" };

        _mockSaveStorageService.Setup(s => s.GetByNameAsync(fileName)).ReturnsAsync(fileObj);

        var result = await _controller.GetFileByName(fileName);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(fileObj, okResult.Value);
    }

    [Fact]
    public async Task GetFileByName_ReturnsNotFound_WhenFileNotExists()
    {
        string fileName = "missing.txt";

        _mockSaveStorageService
            .Setup(s => s.GetByNameAsync(fileName))
            .ReturnsAsync((FileRecord?)null);

        var result = await _controller.GetFileByName(fileName);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

        var data = notFoundResult.Value?.ToString();
        Assert.Contains(fileName, data);
    }
}
