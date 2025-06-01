using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ReferenceDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReferencesController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;

        public ReferencesController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }
        [HttpGet]
        [Route("get-all-references")]
        public async Task<IActionResult> GetAllReferences()
        {
            var response = await _unitOfService.ReferenceService.GetAllReferences();
            return Ok(response);
        }
        [HttpPost("add-ref")]
        //[Authorize(Roles = "Giảng viên")]
        public async Task<IActionResult> AddNewReference(AddReferenceDto request)
        {
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
            var response = await _unitOfService.ReferenceService.AddNewReference(request);
            return Ok(response);
        }
        [HttpPut]
        [Route("update-reference")]
        //[Authorize(Roles = "Giảng viên")]
        public async Task<IActionResult> UpdateReference(UpdateReference request)
        {
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
            var response = await _unitOfService.ReferenceService.UpdateReference(request);
            return Ok(response);
        }
        [HttpDelete]
        [Route("delete-reference")]
        //[Authorize(Roles = "Giảng viên")]
        public async Task<IActionResult> DeleteReference(Delete request)
        {
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
            var response = await _unitOfService.ReferenceService.DeleteReference(request);
            return Ok(response);
        }
        [HttpGet]
        [Route("get-reference-details-id-{referenceId}")]

        public async Task<IActionResult> GetReference(Guid referenceId)
        {
            var response = await _unitOfService.ReferenceService.GetReference(referenceId);
            return Ok(response);
        }
    }
}
