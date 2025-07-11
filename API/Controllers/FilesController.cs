using Application.DTOs.Common;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IStorageService _storageService;

        public FilesController(IConfiguration configuration, IStorageService storageService)
        {
            _configuration = configuration;
            _storageService = storageService;
        }
        /*
        private readonly IAzureBlobIntegration _azureBlobIntegration;

        public FileController(IAzureBlobIntegration azureBlobIntegration)
        {
            _azureBlobIntegration = azureBlobIntegration;
        }

        [HttpPost("upload")]
        public IActionResult UploadFile(IFormFile file)
        {
            var (blobName, uri) = _azureBlobIntegration.Upload("familia", file);
            return Ok(new { blobName, uri });
        }

        [HttpDelete("delete/{blobName}")]
        public IActionResult DeleteFile(string blobName)
        {
            _azureBlobIntegration.Delete("familia", blobName);
            return NoContent();
        }
        */

        // MultipartBodyLengthLimit  was needed for big files with form data.
        // [DisableRequestSizeLimit] works for the KESTREL server, but not IIS server 
        // for IIS: webconfig... <requestLimits maxAllowedContentLength="102428800" />
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        [DisableRequestSizeLimit]
        [HttpPost]
        public async Task<ActionResult<string>> Post(IFormFile file)
        {
            if (file == null)
            {
                return null;
            }

            int MaxFileSizeInMegaBytes = _configuration.GetValue<int>("Storage:MaxFileSizeInMegaBytes", 30);

            if (file.Length > MaxFileSizeInMegaBytes * 1024 * 1024)
            {
                return BadRequest($"The file size should be less than {MaxFileSizeInMegaBytes} Mb");
            }

            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Value.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            var command = new CreateMediaDto
            {
                FileStream = file.OpenReadStream(),
                FileName = fileName,
                MimeType = file.ContentType
            };
            await _storageService.SaveMediaAsync(file.OpenReadStream(), fileName, file.ContentType);
            var fileUrl = _storageService.GetMediaUrl(fileName);

            return Ok(fileUrl);
        }
    }
}
