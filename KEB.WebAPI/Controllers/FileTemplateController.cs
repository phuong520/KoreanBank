using KEB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileTemplateController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;

        public FileTemplateController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }

        //[HttpGet]
        //[Route("reupload-importquestion-template")]
        //[Authorize(Roles = "R2,R3")]
        //public async Task<IActionResult> CreateTemplate()
        //{
        //    await _unitOfService.FileTemplateService.UploadExcelTemplate();
        //    return Ok();
        //}

        //[HttpGet]
        //[Route("get-template-url-to-download")]
        //[Authorize(Roles = "R2,R3")]
        //public async Task<IActionResult> GetTemplateUrl(bool forMultipleChoice)
        //{
        //    var response = await _unitOfService.FileTemplateService.GetTemplateUrl(forMultipleChoice);
        //    return Ok(response);
        //}

        
    }
}
