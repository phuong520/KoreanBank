using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ExamDTO;
using KEB.Application.DTOs.ImportQuestionTaskDTO;
using KEB.Application.DTOs.QuestionDTO;
using KEB.Application.DTOs.SystemAccessLogDTO;
using KEB.Application.Services;
using KEB.Domain.Enums;
using KEB.WebAPI.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;
        private readonly IHubContext<NotifyHub, INotifyClient> _hubContext;

        public TasksController(IUnitOfService unitOfService, IHubContext<NotifyHub, INotifyClient> hubContext)
        {
            _unitOfService = unitOfService;
            _hubContext = hubContext;
        }
        [HttpGet]
        [Route("get-questions-log")]
        //[Authorize(Roles = "R2,R3")]
        public async Task<IActionResult> ViewImportQuestionHistory(
           Guid? userId,
           Guid? taskId,
           DateTime? lowerTimeBound,
           string? action = "Tạo mới")
        {
            ViewAddQuestionHistory request = new()
            {
                Action = action,
                UserId = userId,
                StartDate = lowerTimeBound,
                TaskId = taskId
            };
            var response = await _unitOfService.AccessLogService.ViewImportQuestionHistory(request);
            return Ok(response);
        }
        [HttpPost]
        [Route("lead-change-question-status")]
        // [Authorize(Roles = "R2")]
        public async Task<IActionResult> ChangeQuestionStatus(ChangeQuestionStatusRequest request)
        {
            try
            {
                request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
                var response = await _unitOfService.QuestionService.TeamLeadChangeQuestionStatus(request);
                if (!response.IsSuccess)
                {

                }
                else
                {
                    var notis = await _unitOfService.NotiService.Get7LatestNoti(request.NotifyTo);
                    var client = _hubContext.Clients.User(request.NotifyTo.ToString());
                    Console.WriteLine($"Mapping UserId: {request.NotifyTo}");
                    Console.WriteLine($"User connections: {string.Join(", ", client)}");
                    await _hubContext.Clients.All.SendLatestNotifications(notis);
                    await client.SendLatestNotifications(notis);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("view-import-question-tasks")]
       //[Authorize(Roles = "R2,R3")]
        public async Task<IActionResult> ViewImportQuestionTask(
           Guid? assigneeId,
           Guid? levelId,
           Guid? questionTypeId,
           AddQuestionStatus? status,
           DateTime? lowerBound,
           DateTime? upperBound)
        {
            ViewTaskRequest request = new()
            {
                AssigneeId = assigneeId,
                Status = status,
                LevelId = levelId,
                QuestionTypeId = questionTypeId,
                LowerBound = lowerBound ?? DateTime.MinValue,
                UpperBound = upperBound ?? DateTime.Now
            };
            APIResponse<TaskGeneralDisplayDTO> response = await _unitOfService.AddQuestionTaskService.GetImportQuestionTask(request);
            return Ok(response);
        }
        [HttpGet]
        [Route("view-task-by-id")]
        //[Authorize(Roles = "R2,R3")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var response = await _unitOfService.AddQuestionTaskService.GetTaskDetail(id);
            return Ok(response);
        }

        [HttpPost]
        [Route("assign-import-question-task")]
      //  [Authorize(Roles = "R2")]
        public async Task<IActionResult> AssignImportQuestionTask(AssignTaskRequest request)
        {
            try
            {
                APIResponse<TaskGeneralDisplayDTO> response = await _unitOfService.AddQuestionTaskService.AssignImportQuestionTask(request);
                if (!response.IsSuccess)
                {

                }
                else
                {
                    var notis = await _unitOfService.NotiService.Get7LatestNoti(request.AssigneeId);
                    await _hubContext.Clients.User(request.AssigneeId.ToString()).SendLatestNotifications(notis);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("change-task-deadline")]
        //[Authorize(Roles = "R2")]
        public async Task<IActionResult> ChangeTaskDeadline(ChangeTaskDeadlineRequest request)
        {
            var response = await _unitOfService.AddQuestionTaskService.ChangeTaskDeadline(request);
            return Ok(response);
        }
        [HttpPost]
        [Route("edit-import-question-task")]
       //[Authorize(Roles = "R2")]
        public async Task<IActionResult> EditImportQuestionTask(EditTaskRequest request)
        {
            var response = await _unitOfService.AddQuestionTaskService.EditImportQuestionTask(request);
            return Ok(response);
        }

        [HttpDelete]
        [Route("delete-import-question-task")]
        //[Authorize(Roles = "R2")]
        public async Task<IActionResult> DeleteImportQuestionTask(Delete request)
        {
            var response = await _unitOfService.AddQuestionTaskService.DeleteImportQuestionTask(request);
            return Ok(response);
        }

        [HttpGet]
        [Route("view-exam-as-task")]
       // [Authorize(Roles = "R2,R3")]
        public async Task<IActionResult> ViewExamAsTask(Guid? userId, Guid? levelId, bool? occured, bool? getHost)
        {
            GetExamAsTaskRequest request = new()
            {
                Host = getHost,
                UserId = userId,
                LevelId = levelId,
                Occured = occured,
            };
            var response = await _unitOfService.ExamService.GetExamAsTask(request);
            return Ok(response);
        }
    }
}
