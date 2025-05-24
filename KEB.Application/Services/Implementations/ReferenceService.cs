using AutoMapper;
using FSAKEB.Application.Extensions.FluentValidationRules.ReferenceValidators;
using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ReferenceDTO;
using KEB.Application.Services.Interfaces;
using KEB.Domain.Entities;
using KEB.Domain.ValueObject;
using KEB.Infrastructure.UnitOfWorks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static KEB.Domain.ValueObject.LogicString;

namespace KEB.Application.Services.Implementations
{
    public class ReferenceService : IReferenceService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ReferenceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponse<ReferenceDisplayDto>> AddNewReference(AddReferenceDto request)
        {
            APIResponse<ReferenceDisplayDto> response = new() { IsSuccess = false };

            var requestedUser = await _unitOfWork.Users.GetUserById(request.CreatedBy);
            if (requestedUser == null || requestedUser.RoleId.ToString().Equals(LogicString.Role.AdminRoleId)
                                        || requestedUser.RoleId.ToString().Equals(LogicString.Role.LecturerRoleId))
            {
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.Message = AppMessages.NO_PERMISSION;
            }
            else
            {
                References newReference = _mapper.Map<References>(request);
                ReferenceValidator validator = new();
                var validateResult = validator.Validate(newReference);
                if (!validateResult.IsValid)
                {
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.Message = string.Join(",", validateResult.Errors);
                    return response;
                }
                try
                {
                    var duplicateRef = await _unitOfWork.References.GetAsync(x => x.ReferenceName.Trim().ToLower().Equals(request.ReferenceName.Trim().ToLower()));
                    if (duplicateRef != null)
                    {
                        response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        response.Result.Add(_mapper.Map<ReferenceDisplayDto>(duplicateRef));
                        response.Message = AppMessages.REFERENCE_EXISTED;
                    }
                    else
                    {
                        string details = $"{requestedUser.UserName} đã tạo một Reference mới: " +
                                    $"{newReference.ReferenceName}, {newReference.ReferenceAuthor} ({newReference.PublishedYear})";
                        await _unitOfWork.References.AddAsync(newReference);
                        await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                        {
                            AccessTime = DateTime.Now,
                            IsSuccess = true,
                            ActionName = string.Format(AccessLogConstant.CREATE_ACTION, "nguồn tham khảo"),
                            TargetObject = nameof(References),
                            UserId = request.CreatedBy,
                            IpAddress = request.IpAddress ?? "",
                            Details = details
                        });
                        response.IsSuccess = true;
                        response.Message = AppMessages.REFERENCE_CREATE_SUCCESS;
                        response.StatusCode = System.Net.HttpStatusCode.Created;
                        response.Result.Add(_mapper.Map<ReferenceDisplayDto>(newReference));
                    }
                }
                catch (Exception ex)
                {
                    response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.Conflict;
                }
            }
            return response;
        }

        public async Task<APIResponse<ReferenceDisplayDto>> DeleteReference(Delete request)
        {
            APIResponse<ReferenceDisplayDto> response = new() { IsSuccess = false };

            var requestedUser = await _unitOfWork.Users.GetUserById(request.RequestedUserId);
            if (requestedUser == null || requestedUser.RoleId.ToString().Equals(LogicString.Role.AdminRoleId) || requestedUser.RoleId.ToString().Equals(LogicString.Role.TeamLeadRoleId))
            {
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.Message = AppMessages.NO_PERMISSION;
                return response;
            }
            var targetRef = await _unitOfWork.References.GetByIdAsync(request.TargetObjectId);
            if (targetRef == null)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                return response;
            }
            try
            {
                var (IsSuccess, RelatedQuestions) = await _unitOfWork.References.DeleteRefAsync(targetRef);
                response.IsSuccess = IsSuccess;
                if (IsSuccess)
                {
                    response.Message = AppMessages.REFERENCE_DELETE_SUCCESS;
                    await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                    {
                        IsSuccess = true,
                        AccessTime = DateTime.Now,
                        ActionName = string.Format(AccessLogConstant.DELETE_ACTION, "Reference"),
                        TargetObject = nameof(References),
                        IpAddress = request.IpAddress ?? "",
                        UserId = request.RequestedUserId,
                        Details = $"{requestedUser.UserName} đã xóa reference: {targetRef.ReferenceName}, " +
                                                                             $"{targetRef.ReferenceAuthor} " +
                                                                             $"({targetRef.PublishedYear})"
                    });
                }
                else
                {
                    response.Message = AppMessages.REFERENCE_DELETE_FAILED;
                }
            }
            catch (Exception e)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response; ;
        }

        public async Task<APIResponse<ReferenceDisplayDto>> GetAllReferences()
        {
            APIResponse<ReferenceDisplayDto> response = new();
            Expression<Func<References, bool>> filter = x => true;
            try
            {
                var result = await _unitOfWork.References.GetAllAsync(filter: filter,
                    includeProperties: "Questions", orderBy: x => x.OrderByDescending(x => x.CreatedBy));
                
                foreach (var item in result)
                {
                    Console.WriteLine($"{item.ReferenceName}: {item.Questions.Count}");
                }

                int count = result.Count();
                if (count == 0)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NoContent;
                    response.Message = AppMessages.NO_CONTENT;
                }
                else
                {
                    response.Message = $"~ {count} ~";
                    response.Result = _mapper.Map<List<ReferenceDisplayDto>>(result);
                }
            }
            catch (Exception)
            {
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return response;
        }

        public async Task<APIResponse<ReferenceDisplayDto>> UpdateReference(UpdateReference request)
        {
            APIResponse<ReferenceDisplayDto> response = new() { IsSuccess = false };

            var requestedUser = await _unitOfWork.Users.GetUserById(request.CreatedBy);
            if (requestedUser == null || requestedUser.RoleId.ToString().Equals(LogicString.Role.AdminRoleId) || requestedUser.RoleId.ToString().Equals(LogicString.Role.TeamLeadRoleId))
            {
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.Message = AppMessages.NO_PERMISSION;
                return response;
            }
            var targetRef = await _unitOfWork.References.GetByIdAsync(request.TargetObjectId);
            if (targetRef == null)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                return response;
            }
            References newReference = _mapper.Map<References>(request);
            ReferenceValidator validator = new();
            var validateResult = validator.Validate(newReference);
            if (!validateResult.IsValid)
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Message = string.Join(",", validateResult.Errors);
                return response;
            }
            var duplicateRef = await _unitOfWork.References.GetAsync(x => x.Id != request.TargetObjectId && (x.ReferenceName.Trim().ToLower().Equals(request.ReferenceName.Trim().ToLower())));
            if (duplicateRef != null)
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Result.Add(_mapper.Map<ReferenceDisplayDto>(duplicateRef));
                //response.Message = $"Trong hệ thống đã tồn tại Reference: {duplicateRef.ReferenceName}, " +
                //                                                        $"{duplicateRef.ReferenceAuthor} " +
                //                                                        $"({duplicateRef.PublishedYear})";
                response.Message = AppMessages.REFERENCE_EXISTED;
                return response;
            }
            await _unitOfWork.BeginTransactionAsync();
            DateTime currentTime = DateTime.Now;
            string changeDetails = $"{requestedUser.UserName} đã chỉnh sửa ";
            bool changed = false;
            if (targetRef.ReferencesLink != request.ReferenceLink)
            {
                changed = true;
                changeDetails += "ReferenceLink, ";
                targetRef.ReferencesLink = request.ReferenceLink;
            }
            if (targetRef.ReferenceName != request.ReferenceName)
            {
                changed = true;
                changeDetails += "ReferenceName, ";
                targetRef.ReferenceName = request.ReferenceName;
            }
            if (targetRef.ReferenceAuthor != request.ReferenceAuthor)
            {
                changed = true;
                changeDetails += "ReferenceAuthor, ";
                targetRef.ReferenceAuthor = request.ReferenceAuthor;
            }
            if (targetRef.Description != request.Description)
            {
                changed = true;
                changeDetails += "Description, ";
                targetRef.Description = request.Description;
            }
            if (targetRef.PublishedYear != request.PublishedYear)
            {
                changed = true;
                changeDetails += "PublishedYear";
                targetRef.PublishedYear = request.PublishedYear;
            }
            if (changed)
            {
                targetRef.UpdatedBy = request.CreatedBy;
                targetRef.UpdatedDate = currentTime;
                try
                {
                    //await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.References.UpdateWithNoCommitAsync(targetRef);
                    await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                    {
                        AccessTime = currentTime,
                        TargetObject = nameof(References),
                        ActionName = string.Format(AccessLogConstant.UPDATE_ACTION, "Reference"),
                        IpAddress = request.IpAddress ?? "",
                        UserId = request.CreatedBy,
                        IsSuccess = true,
                        Details = changeDetails
                    });
                    await _unitOfWork.CommitAsync();
                    response.Result.Add(_mapper.Map<ReferenceDisplayDto>(targetRef));
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = AppMessages.REFERENCE_UPDATE_SUCCESS;
                    response.IsSuccess = true;
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackAsync();
                    response.StatusCode = System.Net.HttpStatusCode.Conflict;
                    response.Result = [];
                    response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                }
            }
            else
            {
                response.StatusCode = System.Net.HttpStatusCode.NoContent;
                response.IsSuccess = true;
                response.Message = AppMessages.NO_CHANGES_DETECTED;
            }
            return response;
        }
        public async Task<APIResponse<ReferenceDisplayDto>> GetReference(Guid referenceId)
        {
            var response = new APIResponse<ReferenceDisplayDto> { IsSuccess = false };
            var reference = await _unitOfWork.References.GetByIdAsync(referenceId);
            if (reference == null)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                return response;
            }
            response.IsSuccess = true;
            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.Result.Add(_mapper.Map<ReferenceDisplayDto>(reference));
            return response;
        }
    }
}
