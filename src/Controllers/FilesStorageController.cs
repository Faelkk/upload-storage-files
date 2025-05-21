using Microsoft.AspNetCore.Mvc;

namespace FilesStorage.Controllers;


[ApiController]
[Route("[controller]")]
public class FilestorageController : Controller
{

    private readonly CloudinaryService _cloudinaryService;

    public FilestorageController(CloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpGet("/hello-world")]
    public IActionResult HelloWorld()
    {
        return Ok(new { message = "Hello World" });
    }



    [HttpPost("/upload-local")]
    public async Task<IActionResult> UploadFilesLocal(List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
            return BadRequest("Nenhum arquivo recebido");

        var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".svg" };
        var permittedContentTypes = new[] { "image/jpeg", "image/png", "image/svg+xml" };

        foreach (var file in files)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext) || !permittedContentTypes.Contains(file.ContentType))
                return BadRequest($"Tipo de arquivo não permitido: {file.FileName}");
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePaths = new List<string>();

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var filePath = Path.Combine(uploadsFolder, file.FileName);
                filePaths.Add(filePath);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
        }

        return Ok(new
        {
            count = files.Count,
            urls = filePaths
        });
    }

    [HttpDelete("/delete-local/{fileName}")]
    public IActionResult DeleteImageLocal(string fileName)
    {

        if (fileName.Contains("..") || Path.GetFileName(fileName) != fileName)
            return BadRequest("Nome de arquivo inválido.");

        if (string.IsNullOrEmpty(fileName))
            return BadRequest("Nome do arquivo não fornecido.");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
        var filePath = Path.Combine(uploadsFolder, fileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound("Arquivo não encontrado.");

        try
        {
            System.IO.File.Delete(filePath);
            return Ok("Arquivo deletado com sucesso.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao deletar o arquivo: {ex.Message}");
        }
    }


    [HttpPost("/upload-cloudinary")]
    public async Task<IActionResult> UploadFilesCloudinary(List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
            return BadRequest("Nenhum arquivo recebido");

        var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".svg" };
        var permittedContentTypes = new[] { "image/jpeg", "image/png", "image/svg+xml" };

        var urls = new List<string>();

        foreach (var file in files)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext) || !permittedContentTypes.Contains(file.ContentType))
                return BadRequest($"Tipo de arquivo não permitido: {file.FileName}");

            var url = await _cloudinaryService.UploadImageAsync(file);
            urls.Add(url);
        }

        return Ok(new
        {
            count = files.Count,
            urls
        });
    }

    [HttpDelete("/delete-cloudinary/{publicId}")]
    public async Task<IActionResult> DeleteImageCloudinary(string publicId)
    {
        if (string.IsNullOrEmpty(publicId))
            return BadRequest("PublicId não fornecido.");

        var success = await _cloudinaryService.DeleteImageAsync(publicId);

        if (!success)
            return NotFound("Imagem não encontrada ou erro ao deletar.");

        return Ok("Imagem deletada com sucesso.");
    }

}