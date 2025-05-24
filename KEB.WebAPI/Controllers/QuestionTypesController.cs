using KEB.Application.DTOs.QuestionTypeDTO;
using KEB.Application.Services;
using KEB.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionTypesController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;

        public QuestionTypesController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }
        [HttpGet]
        [Route("get-all-questiontypes")]
        //[Authorize(Roles = "R2,R3")]
        public async Task<IActionResult> GetAllQuestionTypes(string? nameSearchValue, Skill? skill, bool? isDeleted, Guid? createdUserId, DateTime? fromTime)
        {
            var request = new GetQuestionType { CreatedBy = createdUserId, FromTime = fromTime, IsDeleted = isDeleted, Skill = skill, NameValueToBeSearched = nameSearchValue };
            var response = await _unitOfService.QuestionTypeService.GetAllQuestionTypes(request);

            return Ok(response);
        }

        [HttpGet]
        [Route("get-questiontype-{id}")]
        //[Authorize(Roles = "R2,R3")]
        public async Task<IActionResult> GetQuestionType(Guid id)
        {
            var response = await _unitOfService.QuestionTypeService.GetQuestionType(id);

            return Ok(response);
        }
        [HttpPost]
        [Route("add-questiontype")]
        //[Authorize(Roles = "R2")]
        public async Task<IActionResult> AddQuestionType(QuestionTypeCreateDto questionTypeCreateDTO)
        {
            var response = await _unitOfService.QuestionTypeService
                    .AddQuestionType(questionTypeCreateDTO, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            // Thêm loại câu hỏi mới nên cũng chỉnh sửa lại cái template
           // await _unitOfService.FileTemplateService.UploadExcelTemplate();
            return Ok(response);
        }
        [HttpGet("by-skill/{skill}")]
        public async Task<IActionResult> GetBySkill(Skill skill)
        {
            var response = await _unitOfService.QuestionTypeService.GetQuestionTypesBySkillAsync(skill);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpDelete]
        [Route("delete-questiontype")]
        //[Authorize(Roles = "R2")]
        public async Task<IActionResult> DeleteQuestionType(QuestionTypeDeleteDto request)
        {
            var response = await _unitOfService.QuestionTypeService
                    .DeleteQuestionType(request, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            // Xóa loại câu hỏi nên cũng chỉnh sửa lại cái template
            //await _unitOfService.FileTemplateService.UploadExcelTemplate();
            return Ok(response);
        }

        [HttpPut]
        [Route("edit-questiontype")]
        //[Authorize(Roles = "R2")]
        public async Task<IActionResult> EditQuestionType(QuestionTypeUpdateDto questionTypeUpdateDTO)
        {
            var response = await _unitOfService.QuestionTypeService
                    .UpdateQuestionType(questionTypeUpdateDTO, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            // Chỉnh sửa loại câu hỏi nên cũng chỉnh sửa lại cái template
            //await _unitOfService.FileTemplateService.UploadExcelTemplate();
            return Ok(response);
        }
    }
}
