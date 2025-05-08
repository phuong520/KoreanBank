using KEB.Application.DTOs.LevelTopicDetailDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LevelDetailsController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;

        public LevelDetailsController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }
        [HttpGet("get-detail-by-level-id/{levelId}")]
        public async Task<IActionResult> GetDetailByLevelId(Guid levelId)
        {
            var result = await _unitOfService.LevelDetailService.GetDetailByLevelId(levelId);
            // Kiểm tra nếu không tìm thấy kết quả
            if (result == null || !result.IsSuccess)
            {
                return NotFound(new APIResponse1<DetailDisplayDTO>
                {
                    IsSuccess = false,
                    Message = result?.Message ?? "Không tìm thấy dữ liệu chi tiết theo LevelId."
                });
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("assign-topics-to-level")]
        //[Authorize(Roles = "Giảng viên")]
        public async Task<IActionResult> AssignTopicsToLevel(AddValuesToEntityRequest request)
        {
            if (request == null || request.Values == null || !request.Values.Any())
            {
                return BadRequest(new { Message = "Danh sách chủ đề không được để trống" });
            }

            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
            var response = await _unitOfService.LevelDetailService.AssignTopicsToLevel(request);
            
            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            
            return Ok(response);
        }

        [HttpPost]
        [Route("assign-levels-to-topic")]
        //[Authorize(Roles = "Giảng viên")]
        public async Task<IActionResult> AssignLevelsToTopic(AddValuesToEntityRequest request)
        {
            if (request == null || request.Values == null || !request.Values.Any())
            {
                return BadRequest(new { Message = "Danh sách trình độ không được để trống" });
            }

            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
            var response = await _unitOfService.LevelDetailService.AssignLevelsToTopic(request);
            
            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            
            return Ok(response);
        }

        [HttpDelete]
        [Route("delete-level-detail")]
        //[Authorize(Roles = "Giảng viên")]
        public async Task<IActionResult> DeleteLevelDetail(DeleteDetailRequest request)
        {
            if (request == null || request.LevelId == Guid.Empty || request.TopicId == Guid.Empty)
            {
                return BadRequest(new { Message = "Thông tin trình độ và chủ đề không hợp lệ" });
            }

            var response = await _unitOfService.LevelDetailService.DeleteLevelDetail(request);
            
            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            
            return Ok(response);
        }
    }
}
