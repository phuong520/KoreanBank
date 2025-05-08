using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.LevelDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LevelsController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;

        public LevelsController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }
        [HttpGet]
        [Route("get-all-levels")]
        //[Authorize(Roles = "Giảng viên, Quản trị viên")]
        public async Task<IActionResult> GetAllLevels()
        {
            var response = await _unitOfService.LevelService.GetAllLevels();
            return Ok(response);
        }

        [HttpGet]
        [Route("get-level-has-id-{levelId}")]
       // [Authorize(Roles = "Giảng viên, Quản trị viên")]
        public async Task<IActionResult> GetLevel(Guid levelId)
        {
            var response = await _unitOfService.LevelService.GetLevel(levelId);
            return Ok(response);
        }

        [HttpGet]
        [Route("get-all-levels-details")]
        //[Authorize(Roles = "Giảng viên, Quản trị viên")]
        public async Task<IActionResult> GetDetailsOfAllLevels(bool includeEmptyTopic = false)
        {
            //var response = await _unitOfService.LevelService.GetDetailsOfAllLevels(includeEmptyTopic);
            return Ok();
        }

        [HttpPost]
        [Route("add-new-level")]
        //[Authorize(Roles = "Giảng viên")]
        public async Task<IActionResult> AddLevel(AddLevelRequest request)
        {
            var response = await _unitOfService.LevelService
                .AddLevel(request, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            return Ok(response);
        }

        [HttpDelete]
        [Route("delete-level")]
        //[Authorize(Roles = "Giảng viên")]
        public async Task<IActionResult> DeleteLevel(Delete request)
        {
            var response = await _unitOfService.LevelService
                .DeleteLevel(request, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            return Ok(response);
        }

        [HttpGet]
        [Route("level-topic-details")]
        //[Authorize(Roles = "Giảng viên, Quản trị viên")]
        public async Task<IActionResult> GetLevelNameDashTopic()
        {
            var response = await _unitOfService.LevelService.GetLevelNameDashTopic();
            return Ok(response);
        }

        [HttpPut]
        [Route("rename-level")]
        //[Authorize(Roles = "Giảng viên")]
        public async Task<IActionResult> RenameLevel(RenameLevelRequest request)
        {
            var response = await _unitOfService.LevelService
                .RenameLevel(request, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
            return Ok(response);
        }
    }
}
