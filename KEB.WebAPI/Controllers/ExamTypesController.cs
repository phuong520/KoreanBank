using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ExamTypeDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamTypesController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;

        public ExamTypesController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }
        [HttpGet]
        [Route("get-all-exam-types")]
        //[Authorize(Roles = "Quản lý,Giảng viên")]
        public async Task<IActionResult> GetAllExamTypes()
        {
            var response = await _unitOfService.ExamTypeService.GetExamTypesAsync();
            return Ok(response);
        }

        [HttpGet]
        [Route("view-exam-type-detail")]
        //[Authorize(Roles = "Quản lý,Giảng viên")]
        public async Task<IActionResult> ViewExamTypeDetail(Guid id)
        {
            var response = await _unitOfService.ExamTypeService.GetExamTypeDetails(id);
            return Ok(response);
        }

        [HttpDelete]
        [Route("delete-exam-type")]
        //[Authorize(Roles = "Quản lý")]
        public async Task<IActionResult> DeleteExamType(Delete request)
        {
            var response = await _unitOfService.ExamTypeService.DeleteExamTypeAsync(request);
            return Ok(response);
        }

        [HttpPost]
        [Route("add-new-exam-type")]
        //[Authorize(Roles = "Quản lý")]
        public async Task<IActionResult> AddNewExamType(AddExamTypeRequest request)
        {
            var response = await _unitOfService.ExamTypeService.AddExamTypeAsync(request);
            return Ok(response);
        }

        [HttpPost]
        [Route("edit-exam-type")]
        //[Authorize(Roles = "Quản lý")]
        public async Task<IActionResult> EditExamType(FullEditExamTypeRequest request)
        {
            var response = await _unitOfService.ExamTypeService.EditExamTypeAsync(request);
            return Ok(response);
        }
      
    }
}
