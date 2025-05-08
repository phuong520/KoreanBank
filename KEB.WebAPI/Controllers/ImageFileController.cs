using KEB.Application.Services;
using KEB.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageFileController : ControllerBase
    {
        private readonly IImageFileService _imageFileService;
        public ImageFileController(IImageFileService imageFileService)
        {
            _imageFileService = imageFileService;
        }
        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            await _imageFileService.UploadFileAsync(file);
            return Ok();
        }

        [HttpGet("Download")]
        public async Task<IActionResult> DownLoad(Guid id)
        {
            var fileDto = await _imageFileService.DownloadFileAsync(id);
            if (fileDto == null)
            {
                return NotFound();
            }
            return File(fileDto.FileData, fileDto.ContentType, fileDto.FileName);
        }

    }
}
