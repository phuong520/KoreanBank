using KEB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;

        public StatisticsController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }
       
        [HttpGet]
        [Route("imported-questions-by-status")]
        public async Task<IActionResult> ViewImportedQuestionsStatisticByStatus(
           DateTime? lowerBound,
           DateTime? upperBound)
        {
            var response = await _unitOfService.StatisticsService.StatisticImportedQuestionsByStatus(lowerBound ?? DateTime.MinValue,
                                                                                                           upperBound ?? DateTime.Now);
            return Ok(response);
        }
       

        [HttpGet]
        [Route("exams-by-month")]
        public async Task<IActionResult> ViewExamsStatisticByMonth(
           DateTime? lowerBound,
           DateTime? upperBound)
        {
            var thisYear = DateTime.Now.Year;
            var response = await _unitOfService.StatisticsService.StatisticExamByMonth(lowerBound ?? new DateTime(thisYear, 1, 1),
                                                                                        upperBound ?? DateTime.Now);
            return Ok(response);
        }
      

        [HttpGet]
        [Route("exampapers-by-month")]
        public async Task<IActionResult> ViewExamPapersStatisticByMonth(
            DateTime lowerBound,
            DateTime upperBound)
        {
            var response = await _unitOfService.StatisticsService.StatisticExamPaperByMonth(lowerBound, upperBound);
            return Ok(response);
        }
      

    }
}
