using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ExamPaperDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamPapersController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;

        public ExamPapersController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }

        [HttpGet]
        [Route("get-papers")]
        //[Authorize(Roles = "Quản lý,Giảng viên")]
        public async Task<IActionResult> GetExamPapers(
            Guid? examId,
            Guid? levelId,
            string? nameValueTobeSearched,
            int pageNumber = 0,
            int pageSize = 0)
        {
            await Console.Out.WriteLineAsync(HttpContext.User.Identities.ToString());
            ViewExamPapersListRequest request = new()
            {
                
                ExamId = examId,
                LevelId = levelId,
                NameValueToBeSearched = nameValueTobeSearched,
                PaginationRequest = new Pagination { Size = pageSize, Page = pageNumber },
            };
            var response = await _unitOfService.ExamPaperService.ViewExamPapers(request);
            return Ok(response);
        }

        [HttpGet]
        [Route("gen-papers-for-exam")]
        //[Authorize(Roles = "Quản lý,Giảng viên")]
        public async Task<IActionResult> GeneratePaper(Guid examId, Guid requestedUserId)
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
            var response = await _unitOfService.ExamPaperService.UserGenerateExamPapers(examId, requestedUserId, ipAddress);
            return Ok(response);
        }

        [HttpGet]
        [Route("get-paper-detail")]
        //[Authorize(Roles = "Quản lý,Giảng viên")]
        public async Task<IActionResult> ViewPaperDetail(Guid requestedUserId, Guid paperId)
        {
            var request = new ViewPaperDetailRequest
            {
                PaperId = paperId,
                RequestedUserId = requestedUserId,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1",
            };
            var response = await _unitOfService.ExamPaperService.ViewExamPaperDetail(request);
            return Ok(response);
        }

        [HttpPost]
        [Route("edit-exam-paper")]
        //[Authorize(Roles = "Quản lý,Giảng viên")]
        public async Task<IActionResult> EditExamPaper(EditPaperDetailRequest request)
        {
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
            var response = await _unitOfService.ExamPaperService.EditExamPaper(request);
            return Ok(response);
        }

       // [HttpPost]
       // [Route("edit-exam-paper-ver2")]
       //// [Authorize(Roles = "R2,R3")]
       // public async Task<IActionResult> EditExamPaperVer2(EditPaperDetailRequest request)
       // {
       //     request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
       //     var response = await _unitOfService.ExamPaperService.EditExamPaper(request);
       //     return Ok(response);
       // }

        [HttpGet]
        [Route("view-activities-on-paper")]
        //[Authorize(Roles = "Quản lý,Giảng viên")]
        public async Task<IActionResult> ViewPaperActivities(Guid paperId)
        {
            var response = await _unitOfService.ExamPaperService.ViewActivitiesOnPaper(paperId);
            return Ok(response);
        }

        [HttpPost]
        [Route("leave-comment-on-paper")]
        //[Authorize(Roles = "Quản lý,Giảng viên")]
        public async Task<IActionResult> LeaveCommentOnPaper(LeaveCommentRequest request)
        {
            var response = await _unitOfService.ExamPaperService.LeaveCommentOnPaper(request);
            return Ok(response);
        }

        [HttpPut]
        [Route("mark-paper-as-reviewdone")]
        //[Authorize(Roles = "Quản lý,Giảng viên")]
        public async Task<IActionResult> ReviewDonePaper(Guid requestedUserId, Guid paperId)
        {
            ChangePaperStatusRequest request = new()
            {
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1",
                NewStatus = Domain.Enums.PaperStatus.Done,
                PaperId = paperId,
                RequestedUserId = requestedUserId,
            };
            var response = await _unitOfService.ExamPaperService.ChangePaperStatus(request);
            return Ok(response);
        }
        [HttpPut]
        [Route("mark-paper-as-inreview")]
        //[Authorize(Roles = "Giảng viên")]
        public async Task<IActionResult> MarkPaperAsInReview(Guid requestedUserId, Guid paperId)
        {
            ChangePaperStatusRequest request = new()
            {
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1",
                NewStatus = Domain.Enums.PaperStatus.InReview,
                PaperId = paperId,
                RequestedUserId = requestedUserId,
            };
            var response = await _unitOfService.ExamPaperService.ChangePaperStatus(request);
            return Ok(response);
        }
    }
}
