using AutoMapper;
using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.TopicDTO;
using KEB.Application.Services.Interfaces;
using KEB.Application.Utils;
using KEB.Domain.Entities;
using KEB.Domain.ValueObject;
using KEB.Infrastructure.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static KEB.Domain.ValueObject.LogicString;

namespace KEB.Application.Services.Implementations
{
    public class TopicService : ITopicService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TopicService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponse<TopicDisplayDto>> AddNewTopic(AddTopicDto request)
        {
            APIResponse<TopicDisplayDto> response = new() { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };
         
            User? user;
            try
            {
                user = await _unitOfWork.Users.GetUserById(request.CreatedBy);
                if (user == null || user.RoleId.ToString().Equals(LogicString.Role.AdminRoleId)
                             || user.RoleId.ToString().Equals(LogicString.Role.TeamLeadRoleId))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.Message = AppMessages.NO_PERMISSION;
                    return response;
                }
                if (string.IsNullOrEmpty(request.TopicName))
                {
                    throw new (AppMessages.TOPIC_EMPTY_NAME);
                    
                }
                var duplicate = await _unitOfWork.Topics.GetByName(request.TopicName);
                if (duplicate != null)
                {
                  
                    response.Message = AppMessages.TOPIC_EXISTED;
                    response.StatusCode = HttpStatusCode.Conflict;
                    response.Result.Add(_mapper.Map<TopicDisplayDto>(duplicate)); 
                    return response;
                }
                DateTime currentTime = DateTime.Now;
                Topic newTopic = new()
                {
                    TopicName = request.TopicName.Trim(),
                    Description = request.Description.Trim(),
                    CreatedBy = request.CreatedBy,
                    CreatedDate = currentTime,
                    IsDeleted = false,
                    LevelDetails = []
                };
                var levels = request.Levels?.Select(x => new LevelDetail
                {
                    LevelId = x,
                    TopicId = newTopic.Id,
                }).ToList() ?? [];
                if (levels.Count == 0)
                {
                    response.Message = AppMessages.TOPIC_NOLEVEL_SELECTED;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return response;
                }
                // if (levels.Count == 0) throw new BadHttpRequestException(AppMessages.TOPIC_NOLEVEL_SELECTED);
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Topics.AddAsync(newTopic);

                newTopic.LevelDetails = levels;
                await _unitOfWork.AccessLogs.AddAsync(new()
                {
                    AccessTime = currentTime,
                    ActionName = string.Format(AccessLogConstant.CREATE_ACTION, "chủ đề"),
                    TargetObject = nameof(Topic),
                    UserId = request.CreatedBy,
                    IpAddress = request.IpAddress ?? "",
                    IsSuccess = true,
                    Details = $"{user.UserName} tạo chủ đề mới: {newTopic.TopicName}"
                });
                await _unitOfWork.CommitAsync();

                response.IsSuccess = true;
                response.Message = AppMessages.TOPIC_CREATE_SUCCESS;
                response.StatusCode = System.Net.HttpStatusCode.Created;
                response.Result.Add(new TopicDisplayDto()
                {
                    TopicId = newTopic.Id,
                    TopicName = newTopic.TopicName,
                    Description = newTopic.Description,
                });
            }
           
            
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return response;
        }

        public async Task<APIResponse<TopicDisplayDto>> DeleteTopic(Delete request)
        {
            APIResponse<TopicDisplayDto> response = new() { IsSuccess = false };
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.RequestedUserId);
                if (user == null || user.RoleId.ToString().Equals(LogicString.Role.AdminRoleId)
                             || user.RoleId.ToString().Equals(LogicString.Role.TeamLeadRoleId))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.Message = AppMessages.NO_PERMISSION;
                    return response;
                }
                var targetTopic = await _unitOfWork.Topics.GetByIdAsync(request.TargetObjectId);
                if (targetTopic == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                    return response;
                }
                if (!request.HardDelete)
                {
                    targetTopic.IsDeleted = true;
                    targetTopic.UpdatedDate = DateTime.Now;
                    targetTopic.UpdatedBy = request.RequestedUserId;
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                    {
                        AccessTime = DateTime.Now,
                        ActionName = "Tạm ẩn",
                        TargetObject = nameof(Topic),
                        IpAddress = request.IpAddress ?? "",
                        IsSuccess = true,
                        UserId = request.RequestedUserId,
                        Details = $"{user.UserName} đã ẩn chủ đề: {targetTopic.TopicName}"
                    });
                }
                else
                {
                    var (IsSuccess, RelatedQuestions, RelatedLevels, RelatedConstraints) = await _unitOfWork.Topics.DeleteTopicAsync(targetTopic);
                    SystemAccessLog log = new()
                    {
                        AccessTime = DateTime.Now,
                        ActionName = string.Format(AccessLogConstant.DELETE_ACTION, "chủ đề"),
                        TargetObject = nameof(Topic),
                        IpAddress = request.IpAddress ?? "",
                        IsSuccess = IsSuccess,
                        UserId = request.RequestedUserId
                    };
                    if (IsSuccess)
                    {
                        log.Details = $"{user.UserName} đã xóa chủ đề: {targetTopic.TopicName}";
                        await _unitOfWork.AccessLogs.AddAsync(log);
                        response.IsSuccess = true;
                        response.StatusCode = System.Net.HttpStatusCode.OK;
                        response.Message = AppMessages.TOPIC_DELETE_SUCCESS;
                    }
                    else
                    {
                        response.StatusCode = System.Net.HttpStatusCode.Conflict;
                        response.Message = AppMessages.TOPIC_DELETE_FAILED;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return response;
        }

        public async Task<APIResponse<TopicDisplayDto>> GetAllTopics(Guid? levelId = null, bool includedSoftDeleted = false, int page = 1, int size = 10)
        {
            APIResponse<TopicDisplayDto> response = new();
            Expression<Func<Topic, bool>> filter = x => true;
            if (!includedSoftDeleted)
            {
                filter = ExpressionExtension.CombineFilters(filter, x => !x.IsDeleted);
            }
            if (levelId != null)
            {
                filter = ExpressionExtension.CombineFilters(filter, x => x.LevelDetails.Any(x => x.LevelId == levelId));
            }
            try
            {
                var total = await _unitOfWork.Topics.CountAsync(filter);
                ICollection<Topic> topics;
                if (includedSoftDeleted)
                    topics = await _unitOfWork.Topics.GetAllAsync(filter: filter,
                                            includeProperties: "LevelDetails,LevelDetails.Questions,LevelDetails.Questions.QuestionType");
                else
                    topics = await _unitOfWork.Topics.GetAllAsync(filter: filter,
                                            includeProperties: "LevelDetails,LevelDetails.Questions,LevelDetails.Questions.QuestionType");
                var count = topics.Count;
                if (count == 0)
                {
                    response.StatusCode = HttpStatusCode.NoContent;
                    response.Message = AppMessages.NO_CONTENT;
                }
                else
                {
                    response.TotalCount = total;
                    response.Pagination = new Pagination
                    {
                        Page = page,
                        Size = size
                    };
                    response.Message = count + "";
                    response.Result = _mapper.Map<List<TopicDisplayDto>>(topics.OrderByDescending(x => x.CreatedDate).ToList());

                }
            }
            catch (Exception)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;
        }

        public async Task<APIResponse<TopicDisplayDto>> GetTopic(Guid topicId)
        {
            APIResponse<TopicDisplayDto> response = new();
            try
            {
                var topic = await _unitOfWork.Topics.GetAsync(x => x.Id == topicId,
                                                            includeProperties: "LevelDetails,LevelDetails.Questions,LevelDetails.Questions.QuestionType");
                if (topic == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                }
                else
                {
                    response.Result.Add(_mapper.Map<TopicDisplayDto>(topic));
                }
            }
            catch (Exception)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;
        }

        public async Task<APIResponse<TopicDetailDto>> GetTopicDetails(Guid topicId)
        {
            APIResponse<TopicDetailDto> response = new();
            try
            {
                var topic = await _unitOfWork.Topics.GetAsync(x => x.Id == topicId,
                                                            includeProperties: "LevelDetails,LevelDetails.Level," +
                                                                            "LevelDetails.Questions,LevelDetails.Questions.QuestionType");
                if (topic == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                }
                else
                {
                    var detail = _mapper.Map<TopicDetailDto>(topic);
                    response.Result.Add(detail);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;
        }

        public async Task<APIResponse<TopicDisplayDto>> RenameTopic(EditTopicDto request)
        {
            APIResponse<TopicDisplayDto> response = new() { IsSuccess = false };
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.CreatedBy);
                if (user == null || user.RoleId.ToString().Equals(LogicString.Role.AdminRoleId)
                             || user.RoleId.ToString().Equals(LogicString.Role.TeamLeadRoleId))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.Message = AppMessages.NO_PERMISSION;
                    return response;
                }
                if (string.IsNullOrEmpty(request.NewTopicName))
                {
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.Message = AppMessages.TOPIC_EMPTY_NAME;
                    return response;
                }
                var targetTopic = await _unitOfWork.Topics.GetByIdAsync(request.TopicId);
                if (targetTopic == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                    return response;
                }
                string oldName = targetTopic.TopicName;
                bool nameChanged = !request.NewTopicName.Equals(targetTopic.TopicName, StringComparison.OrdinalIgnoreCase);
                bool descChanged = !request.NewDescription.Equals(targetTopic.Description, StringComparison.OrdinalIgnoreCase);
                if (!nameChanged && !descChanged)
                {
                    response.IsSuccess = true;
                    response.StatusCode = System.Net.HttpStatusCode.NoContent;
                    response.Message = AppMessages.NO_CHANGES_DETECTED;
                    return response;
                }
                var duplicate = await _unitOfWork.Topics
                .GetAsync(x => x.Id != request.TopicId && x.TopicName.Trim().ToLower().Equals(request.NewTopicName.Trim().ToLower()));
                if (duplicate != null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    //response.Message = $"Tên chủ đề {request.NewTopicName} đã tồn tại";
                    response.Message = AppMessages.TOPIC_EXISTED;
                }
                else
                {
                    await _unitOfWork.BeginTransactionAsync();
                    DateTime currentTime = DateTime.Now;
                    string logDetails = $"{user.UserName} chỉnh sửa chủ đề {oldName}:"
                                    + (nameChanged ? $" đổi tên chủ đề thành {request.NewTopicName}," : "")
                                    + (descChanged ? " thay đổi mô tả chủ đề" : "");
                    targetTopic.TopicName = request.NewTopicName;
                    targetTopic.Description = request.NewDescription;
                    targetTopic.UpdatedDate = currentTime;
                    targetTopic.UpdatedBy = request.CreatedBy;
                    //await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.Topics.UpdateWithNoCommitAsync(targetTopic);
                    await _unitOfWork.AccessLogs.AddAsync(new()
                    {
                        AccessTime = currentTime,
                        ActionName = string.Format(AccessLogConstant.UPDATE_ACTION, "tên chủ đề"),
                        TargetObject = nameof(Topic),
                        IpAddress = request.IpAddress ?? "",
                        IsSuccess = true,
                        UserId = request.CreatedBy,
                        Details = logDetails
                    });
                    await _unitOfWork.CommitAsync();
                    response.IsSuccess = true;
                    //response.Message = logDetails;
                    response.Message = AppMessages.TOPIC_UPDATE_SUCCESS;

                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;
        }
    }
}
