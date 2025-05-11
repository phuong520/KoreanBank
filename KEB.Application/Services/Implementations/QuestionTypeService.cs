using AutoMapper;
using FSAKEB.Application.Extensions.FluentValidationRules;
using FSAKEB.Application.Extensions.FluentValidationRules.QuestionTypeValidators;
using KEB.Application.DTOs.QuestionTypeDTO;
using KEB.Application.Services.Interfaces;
using KEB.Application.Utils;
using KEB.Domain.Entities;
using KEB.Domain.ValueObject;
using KEB.Infrastructure.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static KEB.Domain.ValueObject.LogicString;

namespace KEB.Application.Services.Implementations
{
    public class QuestionTypeService : IQuestionTypeService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public QuestionTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
             _mapper = mapper;
        }

        public async Task<APIResponse<QuestionTypeDisplayDto>> AddQuestionType(QuestionTypeCreateDto request, string ipAddress)
        {
            APIResponse<QuestionTypeDisplayDto> response = new();
            // CreatedUser existed or not
            var createdUser = await _unitOfWork.Users.GetUserById(request.CreatedUserId);
            if (createdUser == null || createdUser.RoleId.ToString() != LogicString.Role.TeamLeadRoleId)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.Message = AppMessages.NO_PERMISSION;
                return response;
            }
            var accessLog = new SystemAccessLog
            {
                UserId = request.CreatedUserId,
                AccessTime = DateTime.Now,
                ActionName = string.Format(AccessLogConstant.CREATE_ACTION, "loại câu hỏi"),
                TargetObject = nameof(QuestionType),
                IpAddress = ipAddress,
                IsSuccess = false,
                Details = createdUser.UserName + " tạo loại câu hỏi mới: " + request.QuestionTypeName
            };
            // QuestionType fields not null
            var questionTypeValidator = new AddQuestionTypeValidator();
            var validatorResult = questionTypeValidator.Validate(request);
            if (!validatorResult.IsValid)
            {
                response.Message = string.Join(" ", validatorResult.Errors.First().ErrorMessage);
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.IsSuccess = false;
                await _unitOfWork.AccessLogs.AddAsync(accessLog);
                return response;
            }
            // QuestionType not duplicate
            var existedType = await _unitOfWork.QuestionTypes
                                .GetAsync(x => (x.Skill == request.Skill
                                                && (x.TypeName.Trim().ToLower().Equals(request.QuestionTypeName.Trim().ToLower()) || x.TypeContent.Trim().ToLower().Equals(request.QuestionTypeContent.Trim().ToLower()))));
            if (existedType != null)
            {
                response.IsSuccess = false;
                response.Message = AppMessages.QUESTION_TYPE_NAME_EXISTED;
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                await _unitOfWork.AccessLogs.AddAsync(accessLog);
                return response;
            }
            try
            {
                var newQuestionType = _mapper.Map<QuestionType>(request);
                // Perform add
                newQuestionType.CreatedDate = DateTime.Now;
                await _unitOfWork.QuestionTypes.AddAsync(newQuestionType);
                // Access logging
                accessLog.IsSuccess = true;
                await _unitOfWork.AccessLogs.AddAsync(accessLog);
                response.Result.Add(_mapper.Map<QuestionTypeDisplayDto>(newQuestionType));
                // Response
                response.IsSuccess = true;
                response.StatusCode = System.Net.HttpStatusCode.Created;
                response.Message = AppMessages.QUESTION_TYPE_CREATE_SUCCESS;
                response.Result.Add(_mapper.Map<QuestionTypeDisplayDto>(newQuestionType));
                return response;
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                await _unitOfWork.AccessLogs.AddAsync(accessLog);
                return response;
            }
        }

        public Task<APIResponse<QuestionTypeDisplayDto>> DeleteQuestionType(QuestionTypeDeleteDto questionTypeDeleteDTO, string ipAddress)
        {
            throw new NotImplementedException();
        }

        public async Task<APIResponse<QuestionTypeDisplayDto>> GetAllQuestionTypes(GetQuestionType request)
        {
            APIResponse<QuestionTypeDisplayDto> response = new();
            // Base filter
            Expression<Func<QuestionType, bool>> filter = x => true;
            // Combine filters 
            if (request.IsDeleted != null)
            {
                var deletedStatusFilter = (Expression<Func<QuestionType, bool>>)(x => x.IsDeleted == request.IsDeleted);
                filter = ExpressionExtension.CombineFilters(filter, deletedStatusFilter);
            }
            if (request.CreatedBy != null)
            {
                var creatorFilter = (Expression<Func<QuestionType, bool>>)(x => x.CreatedBy.ToString() == (request.CreatedBy).ToString());
                filter = ExpressionExtension.CombineFilters(filter, creatorFilter);
            }
            if (request.Skill != null)
            {
                var skillFilter = (Expression<Func<QuestionType, bool>>)(x => x.Skill == request.Skill);
                filter = ExpressionExtension.CombineFilters(filter, skillFilter);
            }
            if (!string.IsNullOrEmpty(request.NameValueToBeSearched))
            {
                var searchFilter = (Expression<Func<QuestionType, bool>>)(x => x.TypeName.Contains(request.NameValueToBeSearched));
                filter = ExpressionExtension.CombineFilters(filter, searchFilter);
            }
            if (request.FromTime != null)
            {
                var timeFilter = (Expression<Func<QuestionType, bool>>)(x => x.CreatedDate >= request.FromTime);
                filter = ExpressionExtension.CombineFilters(filter, timeFilter);
            }
            // Perform Get
            try
            {
                var queryResult = await _unitOfWork.QuestionTypes.GetAllAsync(filter: filter,
                                        includeProperties: "Questions",
                                        orderBy: src => src.OrderByDescending(item => item.CreatedDate));
                var typesListResult = queryResult.ToList();
                var count = typesListResult.Count;
                if (count == 0)
                {
                    response.IsSuccess = true;
                    response.StatusCode = System.Net.HttpStatusCode.NoContent;
                    response.Message = AppMessages.NO_CONTENT;
                }
                else
                {
                    var finalResult = _mapper.Map<List<QuestionTypeDisplayDto>>(typesListResult);
                    response.Result = finalResult;
                    response.IsSuccess = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = " Tìm thấy " + finalResult.Count + " bản ghi!";
                }
                return response;
            }
            catch (Exception)
            {
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return response;
            }
        }

        public async Task<APIResponse<QuestionTypeDisplayDto>> GetQuestionType(Guid id)
        {
            APIResponse<QuestionTypeDisplayDto> response = new();
            // Perform Get
            try
            {
                var queryResult = await _unitOfWork.QuestionTypes.GetAsync(x => x.Id == id, "Questions");
                if (queryResult == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                }
                else
                {
                    var finalResult = _mapper.Map<QuestionTypeDisplayDto>(queryResult);
                    response.Result.Add(finalResult);
                    response.IsSuccess = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    //response.Message = Common.SUCCESS;
                }
                return response;
            }
            catch (Exception)
            {
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return response;
            }
        }

        public async Task<APIResponse<QuestionTypeDisplayDto>> UpdateQuestionType(QuestionTypeUpdateDto request, string ipAddress)
        {
            APIResponse<QuestionTypeDisplayDto> response = new();
            // Validate
            // QuestionType fields not null
            var questionTypeValidator = new UpdateQuestionTypeValidator();
            var validatorResult = questionTypeValidator.Validate(request);
            if (!validatorResult.IsValid)
            {
                response.Message = string.Join(" ", validatorResult.Errors.First().ErrorMessage);
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.IsSuccess = false;
                return response;
            }
            // CreatedUser existed
            var updatedUser = await _unitOfWork.Users.GetUserById(request.UpdatedUserId);
            if (updatedUser == null || updatedUser.RoleId.ToString() != LogicString.Role.TeamLeadRoleId)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.Message = AppMessages.NO_PERMISSION;
                return response;
            }
            // Target question type existed
            var targetQuestionType = await _unitOfWork.QuestionTypes.GetAsync(x => x.Id == request.QuestionTypeId);
            if (targetQuestionType == null)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                return response;
            }

            List<string> changes = [];
            if (!request.QuestionTypeName.Equals(targetQuestionType.TypeName, StringComparison.OrdinalIgnoreCase))
            {
                changes.Add($"Changes name from {targetQuestionType.TypeName} to {request.QuestionTypeName}");
                targetQuestionType.TypeName = request.QuestionTypeName;
            }
            if (!request.QuestionTypeContent.Equals(targetQuestionType.TypeContent, StringComparison.OrdinalIgnoreCase))
            {
                changes.Add($"Changes content from {targetQuestionType.TypeContent} to {request.QuestionTypeContent}");
                targetQuestionType.TypeContent = request.QuestionTypeContent;
            }
            if (changes.Count == 0)
            {
                response.Message = AppMessages.NO_CHANGES_DETECTED;
                response.StatusCode = System.Net.HttpStatusCode.NoContent;
                return response;
            }
            try
            {
                // QuestionType not duplicate
                var existedType = await _unitOfWork.QuestionTypes
                                    .GetAsync(x => x.Id != targetQuestionType.Id && (x.Skill == targetQuestionType.Skill &&
                                                                                    (x.TypeName.Trim().ToLower().Equals(targetQuestionType.TypeName.Trim().ToLower()) ||
                                                                                     x.TypeContent.Trim().ToLower().Equals(targetQuestionType.TypeContent))));
                if (existedType != null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.Message = AppMessages.QUESTION_TYPE_NAME_EXISTED;
                    return response;
                }

                // Perform update
                targetQuestionType.UpdatedDate = DateTime.Now;
                targetQuestionType.UpdatedBy = request.UpdatedUserId;
                await _unitOfWork.SaveChangesAsync();
                // Access logging
                await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                {
                    UserId = request.UpdatedUserId,
                    AccessTime = DateTime.Now,
                    ActionName = string.Format(AccessLogConstant.UPDATE_ACTION, "loại câu hỏi"),
                    TargetObject = nameof(QuestionType),
                    IpAddress = ipAddress,
                    IsSuccess = true,
                    Details = string.Join("<br/>", changes)
                }
                );
                response.IsSuccess = true;
                response.StatusCode = System.Net.HttpStatusCode.OK;
                response.Message = AppMessages.QUESTION_TYPE_UPDATE_SUCCESS;
                response.Result.Add(_mapper.Map<QuestionTypeDisplayDto>(targetQuestionType));
                return response;
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                return response;
            }
        }
    }
}
