using KEB.Application.DTOs.SystemAccessLogDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessLogsController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;

        public AccessLogsController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }
        [HttpPost]
       // [Authorize(Roles = "R1")]
        public async Task<IActionResult> Get(ViewAccessLog request)
        {
            var response = await _unitOfService.AccessLogService.GetAccessLogs(request);
            return Ok(response);
        }
    }
}
