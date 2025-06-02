using AutoMapper;
using Hangfire.Storage.Monitoring;
using KEB.Application.DTOs.StatisticDTO;
using KEB.Application.Services.Interfaces;
using KEB.Application.Utils;
using KEB.Domain.Entities;
using KEB.Domain.ValueObject;
using KEB.Infrastructure.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Implementations
{
    public class StatisticsService : IStatisticsService
    {
      //  private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public StatisticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //thong ke l exam theo examtype
        public async Task<APIResponse<StatisticDto>> StatisticExamByExamType(DateTime? lowerBound, DateTime? upperBound)
        {
            APIResponse<StatisticDto> response = new();
            Expression<Func<Exam, bool>> filter = x => true; // Init filter ~
            // Combine all filters ~
            {
                if (lowerBound != null)
                {
                    Expression<Func<Exam, bool>> tempFilter = x => x.CreatedDate >= lowerBound;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
                if (upperBound != null)
                {
                    Expression<Func<Exam, bool>> tempFilter = x => x.CreatedDate <= upperBound;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
            }
            try
            {
                var queriedResult = await _unitOfWork.Exams.GetAllAsync(filter, includeProperties: "ExamType");
                var groupedResult = queriedResult.GroupBy(x => x.ExamType);
                response.Result = groupedResult.Select(x => new StatisticDto
                {
                    StatisticName = x.Key.ExamTypeName,
                    StatisticValue = x.Count()
                }).ToList();
                response.Message = "StatisticExamByExamType";
            }
            catch (Exception e)
            {
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return response;
        }

      
        public async Task<APIResponse<StatisticDto>> StatisticExamByMonth(DateTime lowerBound, DateTime upperBound)
        {
            APIResponse<StatisticDto> response = new();
            Expression<Func<Exam, bool>> filter = x => true; // Init filter ~
            if (lowerBound.Year != upperBound.Year)
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Message = "Chỉ thống kê được trong cùng một năm thôi";
                response.IsSuccess = false;
                return response;
            }
            {
                filter = ExpressionExtension.CombineFilters(filter, x => x.CreatedDate >= lowerBound);
                filter = ExpressionExtension.CombineFilters(filter, x => x.CreatedDate <= upperBound);

            }
            try
            {
                var queriedResult = await _unitOfWork.Exams.GetAllAsync(filter);
                var groupedResult = queriedResult.GroupBy(x => x.TakePlaceTime.Month);
                response.Result = groupedResult.Select(x => new StatisticDto
                {
                    StatisticName = new DateTime(lowerBound.Year, x.Key, 1).ToString("MM/yyyy", new CultureInfo("en-US")),
                    StatisticValue = x.Count()
                }).ToList();
                response.Message = "StatisticExamByMonth";
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.Message);
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.IsSuccess = false;
            }
            return response;
        }
        //ok
        public async Task<APIResponse<StatisticDto>> StatisticExamPaperByMonth(DateTime lowerBound, DateTime upperBound)
        {
            APIResponse<StatisticDto> response = new();
            Expression<Func<Exam, bool>> filter = x => true; // Init filter ~

            if (lowerBound.Year != upperBound.Year)
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Message = "Chỉ thống kê được trong cùng một năm thôi";
                response.IsSuccess = false;
                return response;
            }

            filter = ExpressionExtension.CombineFilters(filter, x => x.CreatedDate >= lowerBound);
            filter = ExpressionExtension.CombineFilters(filter, x => x.CreatedDate <= upperBound);

            try
            {
                var queriedResult = await _unitOfWork.Exams.GetAllAsync(filter);
                var groupedResult = queriedResult.GroupBy(x => x.TakePlaceTime.Month)
                                                .ToDictionary(x => x.Key, x => x.Count());

                // Tạo danh sách đầy đủ 12 tháng
                var fullYearResult = new List<StatisticDto>();
                for (int month = 1; month <= 12; month++)
                {
                    var monthValue = groupedResult.ContainsKey(month) ? groupedResult[month] : 0;
                    fullYearResult.Add(new StatisticDto
                    {
                        StatisticName = new DateTime(lowerBound.Year, month, 1).ToString("MM/yyyy", new CultureInfo("en-US")),
                        StatisticValue = monthValue
                    });
                }

                response.Result = fullYearResult;
                response.Message = "StatisticExamByMonth";
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.Message);
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.IsSuccess = false;
            }

            return response;
        }
        //ok
        public async Task<APIResponse<StatisticDto>> StatisticExamPaperBySkill(DateTime lowerBound, DateTime upperBound)
        {
            APIResponse<StatisticDto> response = new();
            Expression<Func<Paper, bool>> filter = x => true; // Init filter ~

            if (lowerBound.Year != upperBound.Year)
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Message = AppMessages.STATISTIC_BY_MONTH;
                response.IsSuccess = false;
                return response;
            }

            filter = ExpressionExtension.CombineFilters(filter, x => x.CreatedDate >= lowerBound);
            filter = ExpressionExtension.CombineFilters(filter, x => x.CreatedDate <= upperBound);

            try
            {
                var queriedResult = await _unitOfWork.Papers.GetAllAsync(filter, includeProperties: "Exam");
                var groupedResult = queriedResult.GroupBy(x => x.Exam.TakePlaceTime.Month)
                                                .ToDictionary(x => x.Key, x => x.Count());

                // Tạo danh sách đầy đủ 12 tháng
                var fullYearResult = new List<StatisticDto>();
                for (int month = 1; month <= 12; month++)
                {
                    var monthValue = groupedResult.ContainsKey(month) ? groupedResult[month] : 0;
                    fullYearResult.Add(new StatisticDto
                    {
                        StatisticName = new DateTime(lowerBound.Year, month, 1).ToString("MM/yyyy", new CultureInfo("en-US")),
                        StatisticValue = monthValue
                    });
                }

                response.Result = fullYearResult;
                response.Message = "StatisticExamPaperByMonth";
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }

            return response;
        }


        public async Task<APIResponse<StatisticDto>> StatisticImportedQuestionsByDifficulty(DateTime? lowerBound, DateTime? upperBound)
        {
            APIResponse<StatisticDto> response = new();
            Expression<Func<Paper, bool>> filter = x => true; // Init filter ~
            // Combine all filters ~
            {
                if (lowerBound != null)
                {
                    Expression<Func<Paper, bool>> tempFilter = x => x.CreatedDate >= lowerBound;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
                if (upperBound != null)
                {
                    Expression<Func<Paper, bool>> tempFilter = x => x.CreatedDate <= upperBound;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
            }
            HashSet<Question> questions = [];
            try
            {
                var papers = await _unitOfWork.Papers.GetAllAsync(filter, includeProperties: "Exam,PaperDetails,PaperDetails.Question");
                foreach (var paper in papers)
                {
                    foreach (var detail in paper.PaperDetails)
                    {
                        questions.Add(detail.Question);
                    }
                }
                var groupedResult = questions.GroupBy(x => x.Difficulty);
                response.Result = groupedResult.Select(x => new StatisticDto
                {
                    StatisticName = x.Key.ToString(),
                    StatisticValue = x.Count()
                }).ToList();
            }
            catch (Exception e)
            {
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return response;
        }
       //ok
        public async Task<APIResponse<StatisticDto>> StatisticImportedQuestionsByStatus(DateTime? lowerBound, DateTime? upperBound)
        {
            APIResponse<StatisticDto> response = new();
            Expression<Func<Question, bool>> filter = x => true; // Init filter ~
            // Combine all filters ~
            {
                if (lowerBound != null)
                {
                    Expression<Func<Question, bool>> tempFilter = x => x.CreatedDate >= lowerBound;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
                if (upperBound != null)
                {
                    Expression<Func<Question, bool>> tempFilter = x => x.CreatedDate <= upperBound;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
            }
            try
            {
                var queriedResult = await _unitOfWork.Questions.GetAllAsync(filter: filter, includeProperties: "LevelDetail,LevelDetail.Topic");
                var groupedResult = queriedResult.GroupBy(x => x.Status);
                response.Result = groupedResult.Select(x => new StatisticDto
                {
                    StatisticName = x.Key.ToString(),
                    StatisticValue = x.Count()
                }).ToList();
                response.Message = "StatisticImportedQuestionsByStatus";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;
        }

        

        public Task<APIResponse<StatisticDto>> StatisticImportQuestionTasks(Guid? assigneeId, DateTime? lowerBound, DateTime? upperBound)
        {
            throw new NotImplementedException();
        }

        public async Task<APIResponse<StatisticDto>> StatisticQuestionsInPapersByDifficulty(DateTime? lowerBound, DateTime? upperBound)
        {
            APIResponse<StatisticDto> response = new();
            Expression<Func<Paper, bool>> filter = x => true; // Init filter ~
            // Combine all filters ~
            {
                if (lowerBound != null)
                {
                    Expression<Func<Paper, bool>> tempFilter = x => x.CreatedDate >= lowerBound;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
                if (upperBound != null)
                {
                    Expression<Func<Paper, bool>> tempFilter = x => x.CreatedDate <= upperBound;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
            }
            HashSet<Question> questions = [];
            try
            {
                var papers = await _unitOfWork.Papers.GetAllAsync(filter, includeProperties: "Exam,PaperQuestions,PaperQuestions.Question");
                foreach (var paper in papers)
                {
                    foreach (var detail in paper.PaperDetails)
                    {
                        questions.Add(detail.Question);
                    }
                }
                var groupedResult = questions.GroupBy(x => x.Difficulty);
                response.Result = groupedResult.Select(x => new StatisticDto
                {
                    StatisticName = x.Key.ToString(),
                    StatisticValue = x.Count()
                }).ToList();
            }
            catch (Exception e)
            {
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return response;
        }

        public async Task<APIResponse<StatisticDto>> StatisticQuestionsInPapersByReference(DateTime? lowerBound, DateTime? upperBound)
        {
            APIResponse<StatisticDto> response = new();
            Expression<Func<Paper, bool>> filter = x => true; // Init filter ~
            // Combine all filters ~
            {
                if (lowerBound != null)
                {
                    Expression<Func<Paper, bool>> tempFilter = x => x.CreatedDate >= lowerBound;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
                if (upperBound != null)
                {
                    Expression<Func<Paper, bool>> tempFilter = x => x.CreatedDate <= upperBound;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
            }
            HashSet<Question> questions = [];
            try
            {
                var papers = await _unitOfWork.Papers.GetAllAsync(filter, includeProperties: "Exam,PaperQuestions,PaperQuestions.Question," +
                                                                    "PaperQuestions.Question.Reference");
                foreach (var paper in papers)
                {
                    foreach (var detail in paper.PaperDetails)
                    {
                        questions.Add(detail.Question);
                    }
                }
                var groupedResult = questions.GroupBy(x => x.References);
                response.Result = groupedResult.Select(x => new StatisticDto
                {
                    StatisticName = x.Key.ReferenceName,
                    StatisticValue = x.Count()
                }).ToList();
            }
            catch (Exception e)
            {
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return response;
        }

        public async Task<APIResponse<StatisticDto>> StatisticQuestionsInPapersByTopic(DateTime? lowerBound, DateTime? upperBound)
        {
            APIResponse<StatisticDto> response = new();
            Expression<Func<Paper, bool>> filter = x => true; // Init filter ~
            // Combine all filters ~
            {
                if (lowerBound != null)
                {
                    Expression<Func<Paper, bool>> tempFilter = x => x.CreatedDate >= lowerBound;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
                if (upperBound != null)
                {
                    Expression<Func<Paper, bool>> tempFilter = x => x.CreatedDate <= upperBound;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
            }
            HashSet<Question> questions = [];
            try
            {
                var papers = await _unitOfWork.Papers.GetAllAsync(filter, includeProperties: "Exam,PaperDetails,PaperDetails.Question," +
                                                                    "PaperDetails.Question.LevelDetail,PaperDetails.Question.LevelDetail.Topic");
                foreach (var paper in papers)
                {
                    foreach (var detail in paper.PaperDetails)
                    {
                        questions.Add(detail.Question);
                    }
                }
                var groupedResult = questions.GroupBy(x => x.LevelDetail.Topic);
                response.Result = groupedResult.Select(x => new StatisticDto
                {
                    StatisticName = x.Key.TopicName,
                    StatisticValue = x.Count()
                }).ToList();
            }
            catch (Exception e)
            {
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return response;
        }
    }
}
