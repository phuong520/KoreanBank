using KEB.Application.DTOs.StatisticDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface IStatisticsService
    {
        Task<APIResponse<StatisticDto>> StatisticExamByExamType(DateTime? lowerBound, DateTime? upperBound);
        Task<APIResponse<StatisticDto>> StatisticExamByMonth(DateTime lowerBound, DateTime upperBound);
        Task<APIResponse<StatisticDto>> StatisticExamPaperByMonth(DateTime lowerBound, DateTime upperBound);
        Task<APIResponse<StatisticDto>> StatisticExamPaperBySkill(DateTime? lowerBound, DateTime? upperBound);
        Task<APIResponse<StatisticDto>> StatisticImportedQuestionsByDifficulty(DateTime? lowerBound, DateTime? upperBound);
        Task<APIResponse<StatisticDto>> StatisticImportedQuestionsByStatus(DateTime? lowerBound, DateTime? upperBound);
        Task<APIResponse<StatisticDto>> StatisticImportQuestionTasks(Guid? assigneeId, DateTime? lowerBound, DateTime? upperBound);
        Task<APIResponse<StatisticDto>> StatisticQuestionsInPapersByDifficulty(DateTime? lowerBound, DateTime? upperBound);
        Task<APIResponse<StatisticDto>> StatisticQuestionsInPapersByReference(DateTime? lowerBound, DateTime? upperBound);
        Task<APIResponse<StatisticDto>> StatisticQuestionsInPapersByTopic(DateTime? lowerBound, DateTime? upperBound);

    }
}
