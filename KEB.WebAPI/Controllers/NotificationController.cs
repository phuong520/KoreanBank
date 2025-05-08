using KEB.Application.DTOs.NotificationDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;
        public NotificationController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }
        [HttpGet("get-7-latest/{userId}")]
        public async Task<ActionResult<List<NotificationDisplayDto>>> Get7LatestNoti(Guid userId)
        {
            try
            {
                var notifications = await _unitOfService.NotiService.Get7LatestNoti(userId);
                if (notifications == null || notifications.Count == 0)
                {
                    return NotFound("No notifications found.");
                }
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("get-all/{userId}")]
        public async Task<ActionResult<List<NotificationDisplayDto>>> GetAllNoti(Guid userId)
        {
            try
            {
                var notifications = await _unitOfService.NotiService.GetAllNoti(userId);
                if (notifications == null || notifications.Count == 0)
                {
                    return NotFound("No notifications found.");
                }
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("mark-all-as-read/{userId}")]
        public async Task<ActionResult> MarkAllNotiAsRead(Guid userId)
        {
            try
            {
                var result = await _unitOfService.NotiService.MarkAllNotiAsRead(userId);
                if (result)
                {
                    return Ok("All notifications marked as read.");
                }
                return NotFound("No unread notifications found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("markasread/{notiId}")]
        public async Task<ActionResult> MarkAsRead(Guid notiId)
        {
            try
            {
                var result = await _unitOfService.NotiService.MarkAsRead(notiId);
                if (result)
                {
                    return Ok("Notification marked as read.");
                }
                return NotFound("Notification not found or already marked as read.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
