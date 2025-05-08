using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using KEB.API.Services;
using KEB.API.Models;

namespace KEB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamPapersController : ControllerBase
    {
        private readonly IExamPaperService _examPaperService;

        public ExamPapersController(IExamPaperService examPaperService)
        {
            _examPaperService = examPaperService;
        }

        [HttpPost("generate-papers")]
        public async Task<IActionResult> GenerateExamPapers([FromBody] GenerateExamPaperRequest request)
        {
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var result = await _examPaperService.GenerateExamPapers(request);
            return StatusCode((int)result.StatusCode, result);
        }
    }
} 