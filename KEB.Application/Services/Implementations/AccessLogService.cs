using AutoMapper;
using KEB.Application.DTOs.SystemAccessLogDTO;
using KEB.Application.Services.Interfaces;
using KEB.Application.Utils;
using KEB.Domain.Entities;
using KEB.Domain.ValueObject;
using KEB.Infrastructure.UnitOfWorks;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Implementations
{
    public class AccessLogService : IAccessLogService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public AccessLogService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponse<AccessLogDisplayDto>> GetAccessLogs(ViewAccessLog request)
        {
            APIResponse<AccessLogDisplayDto> response = new();

            // Base filter
            Expression<Func<SystemAccessLog, bool>> filter = x => true;

            // Combine filters 
            if (request.UserId != null)
            {
                var actorFilter = (Expression<Func<SystemAccessLog, bool>>)(x => x.UserId == request.UserId);
                filter = ExpressionExtension.CombineFilters(filter, actorFilter);
            }
            if (request.IsSuccess != null)
            {
                var successFilter = (Expression<Func<SystemAccessLog, bool>>)(x => x.IsSuccess == request.IsSuccess);
                filter = ExpressionExtension.CombineFilters(filter, successFilter);
            }
            if (!string.IsNullOrEmpty(request.TargetObject))
            {
                var controllerFilter = (Expression<Func<SystemAccessLog, bool>>)(x => x.TargetObject.Contains(request.TargetObject));
                filter = ExpressionExtension.CombineFilters(filter, controllerFilter);
            }
            if (!string.IsNullOrEmpty(request.Action))
            {
                var actionFilter = (Expression<Func<SystemAccessLog, bool>>)(x => x.ActionName.Contains(request.Action));
                filter = ExpressionExtension.CombineFilters(filter, actionFilter);
            }
            {
                Expression<Func<SystemAccessLog, bool>> timeFilter = x => (x.AccessTime >= request.From && x.AccessTime <= request.To);
                filter = ExpressionExtension.CombineFilters(filter, timeFilter);
            }

            try
            {
                // Perform Get
                var allLogs = await _unitOfWork.AccessLogs.GetAllAsync(
                    filter: filter,
                    orderBy: x => x.OrderByDescending(obj => obj.AccessTime),
                    includeProperties: "User,User.Role");

                var logs = allLogs.ToList();

                List<AccessLogDisplayDto> result = _mapper.Map<List<AccessLogDisplayDto>>(logs);
                var count = result.Count;
                if (count == 0)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NoContent;
                    response.Message = AppMessages.NO_CONTENT;
                }
                else
                {
                    response.Result = result;
                    response.Message = "Tìm thấy " + count + " bản ghi";
                };
                return response;
            }
            catch (Exception ex)
            {
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return response;
            }
        }

        public async Task<APIResponse<AddQuestionHistoryDto>> ViewImportQuestionHistory(ViewAddQuestionHistory request)
        {
            APIResponse<AddQuestionHistoryDto> response = new();
            Expression<Func<SystemAccessLog, bool>> filter = x => x.TargetObject.Equals(nameof(Question)) && x.IsSuccess;
            if (request.UserId != null)
            {
                Expression<Func<SystemAccessLog, bool>> userFilter = x => x.UserId == request.UserId;
                filter = ExpressionExtension.CombineFilters(filter, userFilter);
            }
            if (!string.IsNullOrEmpty(request.Action))
            {
                Expression<Func<SystemAccessLog, bool>> actionFilter = x => x.ActionName.ToLower().Contains(request.Action);
                filter = ExpressionExtension.CombineFilters(filter, actionFilter);
            }
            try
            {
                var queriedResult = await _unitOfWork.AccessLogs.GetAllAsync(filter: filter,
                                orderBy: x => x.OrderByDescending(obj => obj.AccessTime),
                                includeProperties: "User");
                if (queriedResult.Count > 0)
                {
                    List<AddQuestionHistoryDto> finalResult = [];
                    foreach (var item in queriedResult)
                    {
                        Expression<Func<Question, bool>> questionFilter = x => x.LogId == item.Id && (request.TaskId == null || x.TaskId == request.TaskId);
                        var relatedQuestions = await _unitOfWork.Questions.GetAllAsync(filter: questionFilter, includeProperties: "Task");
                        //if (relatedQuestions.Count == 0) continue;
                        AddQuestionHistoryDto newDTO = new()
                        {
                            ActionName = item.ActionName,
                            AccessTime = item.AccessTime,
                            Id = item.Id,
                            UserName = item.User?.UserName ?? "",
                            TotalQuestions = relatedQuestions.Count,
                            ApprovedQuestions = relatedQuestions.Count(x => x.Status == Domain.Enums.QuestionStatus.Ok),
                            NeedReviewQuestions = relatedQuestions.Count(x => x.Status == Domain.Enums.QuestionStatus.Pending),
                            DuplicatedQuestions = relatedQuestions.Count(x => x.Status == Domain.Enums.QuestionStatus.Duplicated),
                            DeniedQuestions = relatedQuestions.Count(x => x.Status == Domain.Enums.QuestionStatus.Denied),
                            TaskName = relatedQuestions.FirstOrDefault()?.AddQuestionTask?.TaskName ?? ""
                        };
                        finalResult.Add(newDTO);
                    }
                    response.Message = $"{finalResult.Count}";
                    response.Result = finalResult;
                }
                else
                {
                    response.StatusCode = System.Net.HttpStatusCode.NoContent;
                    response.Message = AppMessages.NO_CONTENT;
                }
            }
            catch (Exception)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.IsSuccess = false;
            }
            return response;
        }
    }
}
