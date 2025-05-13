using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.QuestionAddDTO;
using KEB.Application.DTOs.QuestionDTO;
using KEB.Application.DTOs.SystemAccessLogDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;

        public QuestionsController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }

        [HttpPost]
        [Route("get-questions")]
        //[Authorize(Roles = "R2,R3")]
        public async Task<IActionResult> GetQuestionsAsync(GetQuestionsRequest request)
        {
            var response = await _unitOfService.QuestionService.GetQuestionsListAsync(request);
            return Ok(response);
        }
        [HttpGet]
        [Route("get-question-has-id-{id}")]
        //[Authorize(Roles = "R2,R3")]
        public async Task<IActionResult> GetQuestionAsync(Guid id)
        {
            var response = await _unitOfService.QuestionService.GetQuestionDetailAsync(id);

            return Ok(response);
        }

        [HttpGet]
        [Route("view-import-questions-history")]
        //[Authorize(Roles = "R2,R3")]
        public async Task<IActionResult> ViewQuestionImportHistory(
                Guid? userId,
                bool? isSuccess,
                DateTime? from,
                DateTime? to
            )
        {
            var response = await _unitOfService.AccessLogService.GetAccessLogs(new ViewAccessLog
            {
                TargetObject = "Questions",
                Action = "",
                UserId = userId,
                From = from ?? DateTime.MinValue,
                To = to ?? DateTime.UtcNow,
                IsSuccess = isSuccess,
            });

            return Ok(response);
        }
        [HttpDelete]
        [Route("delete-question")]
        //[Authorize(Roles = "R2,R3")]
        public async Task<IActionResult> DeleteQuestion(Delete request)
        {
            var response = await _unitOfService.QuestionService.TeamLeadDeleteQuestion(request);
            return Ok(response);
        }

        [HttpPost]
        [Route("edit-question")]
        //[Authorize(Roles = "R2,R3")]
        public async Task<IActionResult> EditQuestion([FromForm] UpdateQuestionRequest request)
        {
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
            var response = await _unitOfService.QuestionService.UpdateQuestion(request);
            return Ok(response);
        }
        [HttpPost]
        [Route("add-single-question")]
        //[Authorize(Roles = "R2,R3")]
        public async Task<IActionResult> AddSingleQuestion([FromForm] AddSingleQuestionRequest request)
        {
           
            var result = await _unitOfService.QuestionService.AddSingleQuestionAsync(request);

            if (!result.IsSuccess)
            {
                return StatusCode((int)result.StatusCode, result);
            }

            return Ok(result);
        }
        //[HttpPost]
        //[Route("add-multiple-questions")]
        //[Authorize(Roles = "R2,R3")]
        //public async Task<IActionResult> AddMultipleQuestions([FromForm] ImportQuestionByWordRequest request)
        //{
        //    //var respone = await _unitOfService.QuestionComplexService
        //    //        .UploadQuestionFromWord(request, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");

        //    //return Ok(respone);
        //}
    }
}
