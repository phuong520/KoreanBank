using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ExamDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;

        public ExamsController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }

        [HttpGet]
        [Route("get-exam-by-id")]
        //[Authorize(Roles = "Quản lý,Giảng viên")]
        public async Task<IActionResult> GetExams(Guid? id = null)
        {
            var response = await _unitOfService.ExamService.GetExamAsync(id);

            return Ok(response);
        }

        [HttpPost]
        [Route("add-exam")]
        //[Authorize(Roles = "Quản lý")]
        public async Task<IActionResult> AddExam(AddExamRequest request)
        {
            var response = await _unitOfService.ExamService.AddExamAsync(request);
            return Ok(response);
        }
        [HttpPost]
        [Route("edit-exam")]
        //[Authorize(Roles = "Quản lý")]
        public async Task<IActionResult> EditExam(EditExamRequest request)
        {
            var response = await _unitOfService.ExamService.EditExamAsync(request);
            return Ok(response);
        }

        [HttpDelete]
        [Route("delete-exam")]
        //[Authorize(Roles = "Quản lý")]
        public async Task<IActionResult> DeleteExam([FromForm] Guid requestedUserId, [FromForm] Guid examId)
        {
            Delete request = new()
            {
                HardDelete = true,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1",
                RequestedUserId = requestedUserId,
                TargetObjectId = examId
            };
            var response = await _unitOfService.ExamService.DeleteExamAsync(request);
            return Ok(response);
        }
        
    }
}
