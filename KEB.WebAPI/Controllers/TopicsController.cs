using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.TopicDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;

        public TopicsController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTopics(Guid? levelId, bool includeSoftDeleted = false)
        {
            var response = await _unitOfService.TopicService.GetAllTopics(levelId, includeSoftDeleted);
            return Ok(response);
        }
        [HttpGet]
        [Route("get-topic-id-{topicId}")]
        public async Task<IActionResult> GetTopic(Guid topicId)
        {
            var response = await _unitOfService.TopicService.GetTopic(topicId);
            return Ok(response);
        }

        [HttpGet]
        [Route("get-topic-details-id-{topicId}")]
       // [Authorize(Roles = "R2,R3")]
        public async Task<IActionResult> GetTopicDetails(Guid topicId)
        {
            var response = await _unitOfService.TopicService.GetTopicDetails(topicId);
            return Ok(response);
        }

        [HttpPost("add-new-topic")]
        //[Authorize(Roles = "Giảng viên")]
        public async Task<IActionResult> AddNewTopic(AddTopicDto request)
        {
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
            var response = await _unitOfService.TopicService.AddNewTopic(request);
           // await _unitOfService.FileTemplateService.UploadExcelTemplate();
            return Ok(response);
        }

        [HttpPut]
        [Route("edit-topic")]
        //[Authorize(Roles = "Giảng viên")]
        public async Task<IActionResult> RenameTopic(EditTopicDto request)
        {
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
            var response = await _unitOfService.TopicService.RenameTopic(request);
           // await _unitOfService.FileTemplateService.UploadExcelTemplate("Rename topic");
            return Ok(response);
        }
        [HttpDelete]
        [Route("delete-topic")]
        //[Authorize(Roles = "Giảng viên")]
        public async Task<IActionResult> DeleteTopic(Delete request)
        {
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
            var response = await _unitOfService.TopicService.DeleteTopic(request);
            //await _unitOfService.FileTemplateService.UploadExcelTemplate("Delete topic");
            return Ok(response);
        }

    }
}
