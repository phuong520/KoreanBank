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
        //[HttpGet]
        //[Route("import-question-tasks")]
        ////[Authorize(Roles = "R2,R3")]
        //public async Task<IActionResult> ViewImportQuestionTaskStatistic(
        //   Guid? assigneeId,
        //   DateTime? lowerBound,
        //   DateTime? upperBound)
        //{
        //    var response = await _unitOfService.StatisticsService.StatisticImportQuestionTasks(assigneeId, lowerBound ?? DateTime.MinValue,
        //                                                                                                   upperBound ?? DateTime.Now);
        //    return Ok(response);
        //}
        [HttpGet]
        [Route("imported-questions-by-status")]
        // [Authorize(Roles = "R2")]
        public async Task<IActionResult> ViewImportedQuestionsStatisticByStatus(
           DateTime? lowerBound,
           DateTime? upperBound)
        {
            var response = await _unitOfService.StatisticsService.StatisticImportedQuestionsByStatus(lowerBound ?? DateTime.MinValue,
                                                                                                           upperBound ?? DateTime.Now);
            return Ok(response);
        }
        // [HttpGet]
        // [Route("imported-questions-by-difficulty")]
        //// [Authorize(Roles = "R2")]
        // public async Task<IActionResult> ViewImportedQuestionsStatisticByDifficulty(
        //    DateTime? lowerBound,
        //    DateTime? upperBound)
        // {
        //     var response = await _unitOfService.StatisticsService.StatisticImportedQuestionsByDifficulty(lowerBound ?? DateTime.MinValue,
        //                                                                                                    upperBound ?? DateTime.Now);
        //     return Ok(response);
        // }

        [HttpGet]
        [Route("exams-by-month")]
       // [Authorize(Roles = "R2")]
        public async Task<IActionResult> ViewExamsStatisticByMonth(
           DateTime? lowerBound,
           DateTime? upperBound)
        {
            var thisYear = DateTime.Now.Year;
            var response = await _unitOfService.StatisticsService.StatisticExamByMonth(lowerBound ?? new DateTime(thisYear, 1, 1),
                                                                                        upperBound ?? DateTime.Now);
            return Ok(response);
        }
       // [HttpGet]
       // [Route("exams-by-examtype")]
       //// [Authorize(Roles = "R2")]
       // public async Task<IActionResult> ViewExamsStatisticByExamType(
       //    DateTime? lowerBound,
       //    DateTime? upperBound)
       // {
       //     var response = await _unitOfService.StatisticsService.StatisticExamByExamType(lowerBound ?? DateTime.MinValue,
       //                                                                                     upperBound ?? DateTime.Now);
       //     return Ok(response);
       // }
       // [HttpGet]
       // [Route("exampapers-by-skill")]
       //// [Authorize(Roles = "R2")]
       // public async Task<IActionResult> ViewExamPapersStatisticBySkill(
       //    DateTime? lowerBound,
       //    DateTime? upperBound)
       // {
       //     var response = await _unitOfService.StatisticsService.StatisticExamPaperBySkill(lowerBound ?? DateTime.MinValue,
       //                                                                                     upperBound ?? DateTime.Now);
       //     return Ok(response);
       // }

        [HttpGet]
        [Route("exampapers-by-month")]
        //[Authorize(Roles = "R2")]
        public async Task<IActionResult> ViewExamPapersStatisticByMonth(
            DateTime lowerBound,
            DateTime upperBound)
        {
            var response = await _unitOfService.StatisticsService.StatisticExamPaperByMonth(lowerBound, upperBound);
            return Ok(response);
        }
       // [HttpGet]
       // [Route("questions-in-exampapers-by-difficulty")]
       //// [Authorize(Roles = "R2")]
       // public async Task<IActionResult> ViewQuestionsInExamPapersStatisticByDifficulty(
       //    DateTime? lowerBound,
       //    DateTime? upperBound)
       // {
       //     var response = await _unitOfService.StatisticsService.StatisticQuestionsInPapersByDifficulty(lowerBound, upperBound);
       //     return Ok(response);
       // }
       // [HttpGet]
       // [Route("questions-in-exampapers-by-topic")]
       // //[Authorize(Roles = "R2")]
       // public async Task<IActionResult> ViewQuestionsInExamPapersStatisticByTopic(
       //     DateTime? lowerBound,
       //     DateTime? upperBound)
       // {
       //     var response = await _unitOfService.StatisticsService.StatisticQuestionsInPapersByTopic(lowerBound, upperBound);
       //     return Ok(response);
       // }

       // [HttpGet]
       // [Route("questions-in-exampapers-by-reference")]
       //// [Authorize(Roles = "R2")]
       // public async Task<IActionResult> ViewQuestionsInExamPapersStatisticByReference(
       //     DateTime? lowerBound,
       //     DateTime? upperBound)
       // {
       //     var response = await _unitOfService.StatisticsService.StatisticQuestionsInPapersByReference(lowerBound, upperBound);
       //     return Ok(response);
       // }

    }
}
