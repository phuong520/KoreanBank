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
                             || user.RoleId.ToString().Equals(LogicString.Role.LecturerRoleId))
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

        public Task<APIResponse<TopicDisplayDto>> DeleteTopic(Delete request)
        {
            throw new NotImplementedException();
        }

        public async Task<APIResponse<TopicDisplayDto>> GetAllTopics(Guid? levelId = null, bool includedSoftDeleted = false)
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
                    response.StatusCode = System.Net.HttpStatusCode.NoContent;
                    response.Message = AppMessages.NO_CONTENT;
                }
                else
                {
                    //if (levelId != null)
                    //{
                    //    var details = topics.Select(x => x.LevelDetails);
                    //}
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

        public Task<APIResponse<TopicDisplayDto>> RenameTopic(EditTopicDto request)
        {
            throw new NotImplementedException();
        }
    }
}
