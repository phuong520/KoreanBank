using AutoMapper;
using Azure;
using KEB.Application.DTOs.LevelDTO;
using KEB.Application.DTOs.LevelTopicDetailDTO;
using KEB.Application.DTOs.TopicDTO;
using KEB.Application.Services.Interfaces;
using KEB.Domain.Entities;
using KEB.Domain.ValueObject;
using KEB.Infrastructure.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KEB.Domain.ValueObject.LogicString;

namespace KEB.Application.Services.Implementations
{
    public class LevelDetailService : ILevelDetailService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        
        public LevelDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponse<DetailDisplayDTO>> GetDetailByLevelId(Guid levelId) {

            var response = new APIResponse<DetailDisplayDTO>();
            var levelDetail = await _unitOfWork.LevelDetails.GetDetailByLevelId(levelId);
            // Nếu không tìm thấy LevelDetail, trả về lỗi
            if (levelDetail == null)
            {
                response.IsSuccess = false;
                response.Message = "Không tìm thấy chi tiết với LevelId.";
                return response;
            }
           var list = _mapper.Map<List<DetailDisplayDTO>>(levelDetail);

          
            // Trả về kết quả thành công với thông tin chi tiết
            response.IsSuccess = true;
            response.Result = list;
            response.Message = "Thành công";

            return response;

        }
        public async Task<APIResponse<TopicDisplayDto>> AssignLevelsToTopic(AddValuesToEntityRequest request)
        {
            APIResponse<TopicDisplayDto> response = new();
            
            if (request == null || request.Values == null || !request.Values.Any())
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Message = "Danh sách trình độ không được để trống";
                return response;
            }

            var user = await _unitOfWork.Users.GetUserById(request.RequestedUserId);
            if (user == null)
            {
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.Message = AppMessages.NO_PERMISSION;
                return response;
            }

            var targetTopic = await _unitOfWork.Topics.GetAsync(filter: x => x.Id == request.TargetObjectId, includeProperties: "LevelDetails");
            if (targetTopic == null)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                return response;
            }

            var allLevels = _unitOfWork.Levels.GetAllAsync().Result;
            var curLevelsId = targetTopic.LevelDetails.Select(x => x.LevelId);
            DateTime currentTime = DateTime.Now;
            bool added = false;
            bool removed = false;
            List<string> removeDetails = [$"Xóa khỏi trình độ {targetTopic.TopicName} các chủ đề:"];
            List<LevelDetail> needDelete = [];
            List<string> addDetails = [$"Thêm cho trình độ {targetTopic.TopicName} các chủ đề:"];
            
            foreach (var item in allLevels)
            {
                if (curLevelsId.Contains(item.Id) && !request.Values.Contains(item.Id))
                {
                    targetTopic.LevelDetails.RemoveAll(x => x.LevelId == item.Id);
                    var detail = await _unitOfWork.LevelDetails.GetAsync(filter: x => x.TopicId == request.TargetObjectId
                                                     && x.LevelId == item.Id, includeProperties: "Questions");
                    if (detail != null) needDelete.Add(detail);
                    removed = true;
                    removeDetails.Add(item.LevelName);
                }
                else if (!curLevelsId.Contains(item.Id) && request.Values.Contains(item.Id))
                {
                    targetTopic.LevelDetails.Add(new LevelDetail { LevelId = item.Id, TopicId = request.TargetObjectId });
                    added = true;
                    addDetails.Add(item.LevelName);
                }
            }

            if (added || removed)
            {
                targetTopic.UpdatedDate = currentTime;
                targetTopic.UpdatedBy = request.RequestedUserId;
                string changedDetails = $"{user.UserName} đã chỉnh sửa chủ đề {targetTopic.TopicName}: "
                        + (added ? string.Join("~", addDetails) : "")
                        + (removed ? string.Join("~", removeDetails) : "");
                try
                {
                    await _unitOfWork.BeginTransactionAsync();
                    var (Success, DeletedRecords) = await _unitOfWork.LevelDetails.RemoveRangeLevelDetail(needDelete);
                    if (!Success && DeletedRecords < 0)
                    {
                        response.Message = "Đã có câu hỏi link đến level và topic này, không cho xóa";
                        response.StatusCode = System.Net.HttpStatusCode.FailedDependency;
                        return response;
                    }
                    await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                    {
                        AccessTime = currentTime,
                        ActionName = string.Format(AccessLogConstant.UPDATE_ACTION, "các trình độ của chủ đề"),
                        TargetObject = nameof(Topic),
                        IpAddress = request.IpAddress ?? "",
                        IsSuccess = true,
                        UserId = request.RequestedUserId,
                        Details = changedDetails
                    });
                    response.Message = changedDetails;
                    await _unitOfWork.CommitAsync();
                    response.IsSuccess = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.Conflict;
                    response.Message = ex.Message;
                }
            }
            else
            {
                response.IsSuccess = true;
                response.StatusCode = System.Net.HttpStatusCode.NoContent;
                response.Message = "Không có thay đổi";
            }
            return response;
        }

        public async Task<APIResponse<LevelDisplayBriefDTO>> AssignTopicsToLevel(AddValuesToEntityRequest request)
        {
            APIResponse<LevelDisplayBriefDTO> response = new();
            
            if (request == null || request.Values == null || !request.Values.Any())
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Message = "Danh sách chủ đề không được để trống";
                return response;
            }

            var user = await _unitOfWork.Users.GetUserById(request.RequestedUserId);
            if (user == null || user.RoleId.ToString().Equals(LogicString.Role.AdminRoleId)
                || user.RoleId.ToString().Equals(LogicString.Role.LecturerRoleId))
            {
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.Message = AppMessages.NO_PERMISSION;
                response.IsSuccess = false;
                return response;
            }

            var targetLevel = await _unitOfWork.Levels.GetAsync(filter: x => x.Id == request.TargetObjectId,
                                        includeProperties: "LevelDetails,LevelDetails.Topic");
            if (targetLevel == null)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                return response;
            }

            var allTopics = _unitOfWork.Topics.GetAllAsync().Result;
            var curTopicsId = targetLevel.LevelDetails.Select(x => x.TopicId).ToList();
            DateTime currentTime = DateTime.Now;
            bool added = false;
            bool removed = false;
            List<string> removeDetails = [$"Xóa khỏi trình độ {targetLevel.LevelName} các chủ đề:"];
            List<LevelDetail> needDelete = [];
            List<string> addDetails = [$"Thêm cho trình độ {targetLevel.LevelName} các chủ đề:"];
            
            foreach (var item in allTopics)
            {
                if (curTopicsId.Contains(item.Id) && !request.Values.Contains(item.Id))
                {
                    targetLevel.LevelDetails.RemoveAll(x => x.TopicId == item.Id);
                    var detail = await _unitOfWork.LevelDetails.GetAsync(filter: x => x.LevelId == request.TargetObjectId
                                                             && x.TopicId == item.Id, includeProperties: "Questions");
                    if (detail != null) needDelete.Add(detail);
                    removed = true;
                    removeDetails.Add(item.TopicName);
                }
                else if (!curTopicsId.Contains(item.Id) && request.Values.Contains(item.Id))
                {
                    targetLevel.LevelDetails.Add(new LevelDetail { TopicId = item.Id, LevelId = request.TargetObjectId });
                    added = true;
                    addDetails.Add(item.TopicName);
                }
            }

            if (added || removed)
            {
                targetLevel.UpdatedDate = currentTime;
                targetLevel.UpdatedBy = request.RequestedUserId;
                string changedDetails = $"{user.UserName} đã chỉnh sửa trình độ {targetLevel.LevelName}: "
                        + (added ? string.Join("~", addDetails) : "")
                        + (removed ? string.Join("~", removeDetails) : "");
                try
                {
                    await _unitOfWork.BeginTransactionAsync();
                    var (Success, DeletedRecords) = await _unitOfWork.LevelDetails.RemoveRangeLevelDetail(needDelete);
                    if (!Success && DeletedRecords < 0)
                    {
                        response.Message = "Đã có câu hỏi link đến level và topic này, không cho xóa";
                        response.StatusCode = System.Net.HttpStatusCode.FailedDependency;
                        return response;
                    }
                    await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                    {
                        AccessTime = currentTime,
                        ActionName = string.Format(AccessLogConstant.UPDATE_ACTION, "các chủ đề của trình độ"),
                        TargetObject = nameof(Level),
                        IpAddress = request.IpAddress ?? "",
                        IsSuccess = true,
                        UserId = request.RequestedUserId,
                        Details = changedDetails
                    });
                    await _unitOfWork.CommitAsync();
                    response.Message = AppMessages.LEVEL_ADDTOPIC_SUCCESS;
                    response.IsSuccess = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.Conflict;
                    response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                }
            }
            else
            {
                response.IsSuccess = true;
                response.StatusCode = System.Net.HttpStatusCode.NoContent;
                response.Message = AppMessages.NO_CHANGES_DETECTED;
            }
            return response;
        }

        public async Task<APIResponse<DetailDisplayDTO>> DeleteLevelDetail(DeleteDetailRequest request)
        {
            APIResponse<DetailDisplayDTO> response = new() { StatusCode = System.Net.HttpStatusCode.BadRequest, IsSuccess = false };
            
            if (request == null || request.LevelId == Guid.Empty || request.TopicId == Guid.Empty)
            {
                response.Message = "Thông tin trình độ và chủ đề không hợp lệ";
                return response;
            }

            var user = await _unitOfWork.Users.GetUserById(request.RequestedUserId);
            if (user == null || user.RoleId.ToString().Equals(LogicString.Role.AdminRoleId)
                || user.RoleId.ToString().Equals(LogicString.Role.LecturerRoleId))
            {
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.Message = Permission.NoPermission;
                return response;
            }

            var level = await _unitOfWork.Levels.GetLevelById(request.LevelId);
            if (level == null)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                return response;
            }
            
            var topic = await _unitOfWork.Topics.GetByIdAsync(request.TopicId);
            if (topic == null)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                return response;
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var (Deleted, RelatedQuestion) = await _unitOfWork.LevelDetails.RemoveLevelDetail(request.LevelId, request.TopicId);

                if (Deleted)
                {
                    await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                    {
                        AccessTime = DateTime.Now,
                        ActionName = string.Format(AccessLogConstant.DELETE_ACTION, "mối quan hệ trình độ-chủ đề"),
                        TargetObject = nameof(LevelDetail),
                        IpAddress = request.IpAddress ?? "",
                        IsSuccess = true,
                        UserId = request.RequestedUserId,
                        Details = $"{user.UserName} đã xóa mối quan hệ giữa trình độ {level.LevelName} và chủ đề {topic.TopicName}"
                    });

                    response.Result.Add(new DetailDisplayDTO
                    {
                        //LevelId = request.LevelId,
                        //LevelName = level.LevelName,
                        //TopicId = request.TopicId,
                        TopicName = topic.TopicName
                    });
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = AppMessages.TOPIC_REMOVE_FROM_LEVEL_SUCCESS;
                    response.IsSuccess = true;
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    response.Message = AppMessages.TOPIC_REMOVE_FROM_LEVEL_FAILED;
                    await _unitOfWork.RollbackAsync();
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = ex.Message;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }

            return response;
        }

        public Task<APIResponse<DetailDisplayDTO>> GetDetailByLevelTopic(Guid levelId, Guid topicId)
        {
            throw new NotImplementedException();
        }
    }
}
