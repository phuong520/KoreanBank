using AutoMapper;
using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.LevelDTO;
using KEB.Application.DTOs.LevelTopicDetailDTO;
using KEB.Application.DTOs.TopicDTO;
using KEB.Application.Services.Interfaces;
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
    public class LevelService : ILevelService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public LevelService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponse<LevelDisplayDetailDTO>> AddLevel(AddLevelRequest request, string ipAddress)
        {
            var response = new APIResponse<LevelDisplayDetailDTO>() { StatusCode = System.Net.HttpStatusCode.BadRequest, IsSuccess = false };
            var requestedUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.RequestedUserId, includeProperties: "Role");
            if (requestedUser == null || requestedUser.Id != request.RequestedUserId)
            {
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.Message = AppMessages.NO_PERMISSION;
                return response;
            }
            if (string.IsNullOrEmpty(request.LevelName))
            {
                response.Message = AppMessages.LEVEL_EMPTY_NAME;
                return response;
            }

            var existed = await _unitOfWork.Levels.GetLevelByName(request.LevelName);
            if (existed != null)
            {
                response.Message = AppMessages.LEVEL_EXISTED;
                return response;
            }
            DateTime currentTime = DateTime.Now;
            Level newLevel = new()
            {
                LevelName = request.LevelName,
                CreatedDate = currentTime,
                CreatedBy = request.RequestedUserId,
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Levels.AddAsync(newLevel);

                List<LevelDetail> details = request.Topics?.Select(x => new LevelDetail
                {
                    LevelId = newLevel.Id,
                    TopicId = x,
                }).ToList() ?? [];
                newLevel.LevelDetails = details;

                await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                {
                    ActionName = string.Format(AccessLogConstant.CREATE_ACTION, "trình độ"),
                    TargetObject = AccessLogConstant.LEVEL_CONTROLLER,
                    AccessTime = currentTime,
                    IpAddress = ipAddress,
                    IsSuccess = true,
                    UserId = request.RequestedUserId,
                    Details = $"{requestedUser.UserName} đã tạo trình độ mới: {newLevel.LevelName} với {details.Count} chủ đề"
                });
                await _unitOfWork.CommitAsync();

                response.IsSuccess = true;
                response.Message = AppMessages.LEVEL_CREATE_SUCCESS;
                response.StatusCode = System.Net.HttpStatusCode.Created;
                var finalResult = _mapper.Map<LevelDisplayDetailDTO>(newLevel);
                response.Result.Add(finalResult);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.StatusCode = System.Net.HttpStatusCode.Conflict;
            }
            return response;
        }

        public async Task<APIResponse<LevelDisplayBriefDTO>> DeleteLevel(Delete request, string ipAddress)
        {
            var response = new APIResponse<LevelDisplayBriefDTO>();
            var requestedUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.RequestedUserId, includeProperties: "Role");
            if (requestedUser == null || !requestedUser.Role.RoleName.Contains("Giảng viên"))
            {
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.IsSuccess = false;
                response.Message = AppMessages.NO_PERMISSION;
                return response;
            }
            var targetLevel = await _unitOfWork.Levels.GetLevelById(request.TargetObjectId);
            if (targetLevel == null)
            {
                response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.IsSuccess = false;
            }
            else
            {
                try
                {
                    var (IsSuccess, RelatedQuestions, RelatedExamTypes, RelatedTopics) = await _unitOfWork.Levels.DeleteLevel(request.TargetObjectId);
                    string message = "";
                    var accessLog = new SystemAccessLog
                    {
                        ActionName = string.Format(AccessLogConstant.DELETE_ACTION, "trình độ"),
                        TargetObject = nameof(Level),
                        AccessTime = DateTime.Now,
                        IpAddress = ipAddress,
                        IsSuccess = IsSuccess,
                        UserId = request.RequestedUserId,
                    };
                    if (IsSuccess)
                    {
                        //message = $"{requestedUser.UserName} xóa trình độ: {targetLevel.LevelName}";
                        message = AppMessages.LEVEL_DELETE_SUCCESS;
                    }
                    else
                    {
                        //message = $"{requestedUser.UserName} xóa trình độ: {targetLevel.LevelName} thất bại. Lý do: " +
                        //              $"có {RelatedQuestions} câu hỏi " +
                        //              $"và {RelatedExamTypes} loại kỳ thi liên quan!";
                        message = AppMessages.LEVEL_DELETE_FAILED;
                        response.IsSuccess = false;
                        response.StatusCode = System.Net.HttpStatusCode.Conflict;

                    }
                    accessLog.Details = message;
                    await _unitOfWork.AccessLogs.AddAsync(accessLog);
                    response.Message = message;
                }
                catch (Exception ex)
                {
                    response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                }
            }
            return response;
        }
      
        public async Task<APIResponse<LevelDisplayBriefDTO>> GetAllLevels()
        {
            APIResponse<LevelDisplayBriefDTO> response = new();
            Expression<Func<Level, bool>> filter = x => true;
            try
            {
                ICollection<Level> levels = await _unitOfWork.Levels
                                                .GetAllAsync(
                    filter: filter,
                    includeProperties: "LevelDetails,LevelDetails.Questions,LevelDetails.Questions.QuestionType", orderBy: src => src.OrderByDescending(item => item.CreatedDate));
                if (levels == null || levels.Count == 0)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NoContent;
                    response.Message = AppMessages.NO_CONTENT;
                }
                else
                {
                    response.Result = _mapper.Map<List<LevelDisplayBriefDTO>>(levels.ToList().OrderByDescending(x => x.CreatedDate));
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

        public async Task<APIResponse<LevelDisplayDetailDTO>> GetLevelDetails(Guid levelId)
        {
            APIResponse<LevelDisplayDetailDTO> response = new();
            try
            {
                var level = await _unitOfWork.Levels.GetAsync(filter: x => x.Id == levelId,
                                includeProperties: "LevelDetails," +
                                "LevelDetails.Topic,LevelDetails.Questions," +
                                "LevelDetails.Questions.QuestionType");
                if (level == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                }
                else
                {
                    var details = level.LevelDetails;
                    var result = _mapper.Map<LevelDisplayDetailDTO>(level);
                    result.Topics = _mapper.Map<List<TopicDisplayDto>>(details);
                    response.Result.Add(result);
                }
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return response;
        }

        public async Task<APIResponse<LevelDisplayBriefDTO>> GetLevel(Guid id)
        {
            APIResponse<LevelDisplayBriefDTO> response = new();
            try
            {
                Level? level = await _unitOfWork.Levels.GetAsync(filter: x => x.Id == id,
                                                                includeProperties: "LevelDetails,LevelDetails.Questions,LevelDetails.Questions.QuestionType");
                if (level == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                }
                else
                {
                    response.Result.Add(_mapper.Map<LevelDisplayBriefDTO>(level));
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<APIResponse<DetailDisplayDTO>> GetLevelNameDashTopic()
        {
            var response = new APIResponse<DetailDisplayDTO>();
            try
            {
                var levelDetails = await _unitOfWork.LevelDetails.GetAllAsync(
                    includeProperties: "Level,Topic"
                );
                if (levelDetails == null || !levelDetails.Any())
                {
                    response.StatusCode = System.Net.HttpStatusCode.NoContent;
                    response.Message = AppMessages.NO_CONTENT;
                }
                else
                {
                    var details = levelDetails.Select(ld => new DetailDisplayDTO
                    {
                        DetailId = ld.Id,
                        //LevelId = ld.LevelId,
                        //LevelName = ld.Level.LevelName,
                        //TopicId = ld.TopicId,
                        TopicName = ld.Topic.TopicName
                    }).ToList();
                    response.Result = details;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<APIResponse<LevelDisplayBriefDTO>> RenameLevel(RenameLevelRequest request, string ipAddress)
        {
            var response = new APIResponse<LevelDisplayBriefDTO>();
            var requestedUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.RequestedUserId, includeProperties: "Role");
            if (requestedUser == null || !requestedUser.Role.RoleName.Contains("Giảng viên"))
            {
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.IsSuccess = false;
                response.Message = AppMessages.NO_PERMISSION;
                return response;
            }
            if (string.IsNullOrEmpty(request.NewLevelName))
            {
                response.Message = AppMessages.LEVEL_EMPTY_NAME;
                return response;
            }
            var targetLevel = await _unitOfWork.Levels.GetLevelById(request.TargetLevelId);
            if (targetLevel == null)
            {
                response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.IsSuccess = false;
                return response;
            }
            var existed = await _unitOfWork.Levels.GetLevelByName(request.NewLevelName);
            if (existed != null)
            {
                response.Message = AppMessages.LEVEL_EXISTED;
                return response;
            }
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                targetLevel.LevelName = request.NewLevelName;
                await _unitOfWork.Levels.UpdateWithNoCommitAsync(targetLevel);
                await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                {
                    ActionName = string.Format(AccessLogConstant.UPDATE_ACTION, "trình độ"),
                    TargetObject = AccessLogConstant.LEVEL_CONTROLLER,
                    AccessTime = DateTime.Now,
                    IpAddress = ipAddress,
                    IsSuccess = true,
                    UserId = request.RequestedUserId,
                    Details = $"{requestedUser.UserName} đã đổi tên trình độ: {targetLevel.LevelName}"
                });
                await _unitOfWork.CommitAsync();
                response.IsSuccess = true;
                response.Message = AppMessages.LEVEL_UPDATE_SUCCESS;
                response.StatusCode = System.Net.HttpStatusCode.OK;
                response.Result.Add(_mapper.Map<LevelDisplayBriefDTO>(targetLevel));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
            }
            return response;
        }
    }
}
