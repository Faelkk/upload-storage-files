using FilesStorage.Dtos;
using FilesStorage.Messages;
using FilesStorage.Services;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace FilesStorage.Controllers;

[ApiController]
[Route("[controller]")]
public class FilestorageController : Controller
{
    private readonly IFileLocalService _fileLocalService;
    private readonly IFileCloudService _fileCloudService;
    private readonly ISaveStorageService _saveStorageService;

    public FilestorageController(
        IFileLocalService fileLocalService,
        IFileCloudService fileCloudService,
        ISaveStorageService saveStorageService
    )
    {
        _fileLocalService = fileLocalService;
        _fileCloudService = fileCloudService;
        _saveStorageService = saveStorageService;
    }

    [HttpPost("/upload-local")]
    public async Task<IActionResult> UploadFilesLocal(List<IFormFile> files)
    {
        var (success, error) = await _fileLocalService.UploadFilesAsync(files);
        if (!success)
            return BadRequest(error);
        return Accepted(
            new UploadResponseDto { Count = files.Count, Message = "Upload local em processamento" }
        );
    }

    [HttpDelete("/delete-local/{fileName}")]
    public async Task<IActionResult> DeleteImageLocal(string fileName)
    {
        var (success, error) = await _fileLocalService.DeleteFileAsync(fileName);
        if (!success)
            return BadRequest(error);
        return Ok(
            new DeleteResponseDto
            {
                Message = $"Remoção do arquivo local '{fileName}' em processamento.",
            }
        );
    }

    [HttpPost("/upload-cloud")]
    public async Task<IActionResult> UploadFilesCloud(List<IFormFile> files)
    {
        var (success, error) = await _fileCloudService.UploadFilesAsync(files);
        if (!success)
            return BadRequest(error);
        return Ok(
            new UploadResponseDto
            {
                Count = files.Count,
                Message = "Upload na nuvem em processamento",
            }
        );
    }

    [HttpDelete("/delete-cloud/{publicId}")]
    public async Task<IActionResult> DeleteImageCloud(string publicId, [FromQuery] string fileName)
    {
        var (success, error) = await _fileCloudService.DeleteFileAsync(publicId, fileName);

        if (!success)
            return BadRequest(error);

        return Ok(
            new DeleteResponseDto
            {
                Message = $"Remoção da imagem na nuvem '{publicId}' em processamento.",
            }
        );
    }

    [HttpGet("/files")]
    public async Task<IActionResult> GetAllFiles()
    {
        var files = await _saveStorageService.GetAllAsync();
        return Ok(files);
    }

    [HttpGet("/files/{fileName}")]
    public async Task<IActionResult> GetFileByName(string fileName)
    {
        var file = await _saveStorageService.GetByNameAsync(fileName);
        if (file is null)
            return NotFound(new { message = $"Arquivo '{fileName}' não encontrado." });

        return Ok(file);
    }
}
