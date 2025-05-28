using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.QuestionAddDTO;
using KEB.Application.DTOs.QuestionDTO;
using KEB.Application.DTOs.SystemAccessLogDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;
        private readonly string _templatePath;
        public QuestionsController(IUnitOfService unitOfService, IConfiguration configuration)
        {
            _unitOfService = unitOfService;
            _templatePath = configuration["TemplatePath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates");
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
        //[HttpPost("upload-excel")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> UploadExcel([FromForm] ImportQuestionFromExcelRequest request)
        //{
        //    try
        //    {
        //        // Get RequestedUserId from claims
        //        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var requestedUserId))
        //        {
        //            return Unauthorized(new APIResponse<ImportQuestionResultDTO>
        //            {
        //                IsSuccess = false,
        //                StatusCode = HttpStatusCode.Unauthorized,
        //                Message = "Invalid user authentication."
        //            });
        //        }

        //        request.RequestedUserId = requestedUserId;

        //        // Get client IP address
        //        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        //        // Call service
        //        var response = await _unitOfService.QuestionService.UploadQuestionFromExcel(request, ipAddress);

        //        return StatusCode((int)response.StatusCode, response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<ImportQuestionResultDTO>
        //        {
        //            IsSuccess = false,
        //            StatusCode = HttpStatusCode.InternalServerError,
        //            Message = $"Error processing request: {ex.Message}"
        //        });
        //    }
        //}

        [HttpGet("download-template")]
        public async Task<IActionResult> DownloadTemplate(bool forMultipleChoice = true)
        {
            var bytes = await _unitOfService.QuestionWithFileService.UploadExcelTemplate(forMultipleChoice);
            var fileName = forMultipleChoice ? "multiple_choice_template.xlsx" : "essay_template.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        [HttpPost("upload-excel")]
        public async Task<IActionResult> UploadExcel([FromForm] ImportQuestionFromExcelRequest request)
        {
            if (request.ExcelFile == null || request.ExcelFile.Length == 0)
            {
                return BadRequest(new APIResponse<ImportQuestionResultDTO>
                {
                    IsSuccess = false,
                    Message = "Vui lòng chọn file Excel",
                    StatusCode = HttpStatusCode.BadRequest,
                    Result = new List<ImportQuestionResultDTO>
                {
                    new ImportQuestionResultDTO
                    {
                        Messages = new List<string> { "Vui lòng chọn file Excel" }
                    }
                }
                });
            }
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
            var response = await _unitOfService.QuestionService.UploadQuestionFromExcel(request, ipAddress);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
