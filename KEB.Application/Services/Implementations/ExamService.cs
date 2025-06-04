using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ExamDTO;
using KEB.Application.Services.Interfaces;
using KEB.Application.Utils;
using KEB.Domain.Entities;
using KEB.Domain.ValueObject;
using KEB.Infrastructure.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Implementations
{
    public class ExamService : IExamService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ExamService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponse<object>> AddExamAsync(AddExamRequest request)

        {
            APIResponse<object> response = new() { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };

            User? requestedUser;
            try
            {
                requestedUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.RequestedUserId,
                                                                includeProperties: "Role");
                if (requestedUser == null || (requestedUser.RoleId.ToString() != LogicString.Role.TeamLeadRoleId))
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.Message = AppMessages.NO_PERMISSION;
                    return response;
                }
            }
            catch (Exception)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                return response;
            }
            DateTime currentTime = DateTime.Now;
            DateTime earliestTakePlaceTime = currentTime.AddDays(SystemDataFormat.EARLIEST_EXAM_TAKEPLACETIME_FROMNOW);
            List<string> validateRequestResult = [];
            var examType = await _unitOfWork.ExamTypes.GetAsync(filter: x => x.Id == request.ExamTypeId,
                                                                includeProperties: "Levels");
            if (examType == null)
                validateRequestResult.Add("Không tìm thấy loại kỳ thi");

            if (string.IsNullOrEmpty(request.ExamName))
                validateRequestResult.Add("Tên kỳ thi không được để trống");

            if (request.TakePlaceTime < earliestTakePlaceTime)
                validateRequestResult.Add($"Ngày thi của kỳ thi mới phải cách bây giờ " +
                            $"ít nhất {SystemDataFormat.EARLIEST_EXAM_TAKEPLACETIME_FROMNOW} ngày");

            var host = await _unitOfWork.Users.GetByIdAsync(request.HostId);
            if (host == null || !host.IsActive || host.RoleId.ToString() == LogicString.Role.AdminRoleId)
                validateRequestResult.Add("Người dùng này không thể nhận vai trò làm host cho kỳ thi ~");
            //else if (host.RoleId.ToString() == LocalizationString.Role.AdminRoleId)
            //    validateRequestResult.Add("Bạn không đủ trình giao nhiệm vụ cho admin đâu ~");

            var reviewer = await _unitOfWork.Users.GetByIdAsync(request.ReviewerId);
            if (reviewer == null || !reviewer.IsActive || reviewer.RoleId.ToString() == LogicString.Role.AdminRoleId)
                validateRequestResult.Add("Người dùng này không thể nhận vai trò reviewer cho kỳ thi ~");
            //else if (reviewer.RoleId.ToString() != LocalizationString.Role.LecturerRoleId)
            //    validateRequestResult.Add("Bạn không đủ trình giao nhiệm vụ cho admin đâu ~");

            if (request.HostId == request.ReviewerId)
                validateRequestResult.Add("Host và Reviewer phải là 2 người khác nhau");

            if (validateRequestResult.Count > 0)
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Message = "<strong>Kỳ thi mới có nhiều điểm không hợp lệ</strong> ~ <br/>" + string.Join(". ", validateRequestResult);
                response.Result = validateRequestResult.Select(x => (object)x).ToList();
            }
            else
            {
                Exam newExam = new()
                {
                    CreatedBy = request.RequestedUserId,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    ExamName = request.ExamName,
                    ExamTypeId = request.ExamTypeId,
                    TakePlaceTime = request.TakePlaceTime,
                    IsSuspended=false,
                    IsDeleted = false,
                    ReviewerId = request.ReviewerId,
                    HostId = request.HostId
                };
                try
                {
                    await _unitOfWork.Exams.AddAsync(newExam);
                    await _unitOfWork.AccessLogs.AddAsync(new()
                    {
                        AccessTime = currentTime,
                        ActionName = $"Tạo kỳ thi mới",
                        TargetObject = $"Loại kỳ thi: {examType.ExamTypeName}",
                        IpAddress = request.IpAddress ?? "::1",
                        IsSuccess = true,
                        UserId = requestedUser.Id,
                        Details = $"Tạo kỳ thi mới: {newExam.ExamName} cho loại kỳ thi {examType.ExamTypeName}"
                    });
                    List<SendEmail> emails = [
                        new SendEmail{
                            Subject = string.Format(SystemDataFormat.EXAM_NEWRELEVANCE_EMAIL_SUBJECT, "HOST"),
                            Body = string.Format(SystemDataFormat.EXAM_NEWRELEVANCE_EMAIL_BODY,
                                            host.UserName,
                                            requestedUser.UserName,
                                            "host",
                                            newExam.ExamName,
                                            examType.ExamTypeName,
                                            newExam.ExamName,
                                            examType.Levels.LevelName,
                                            newExam.TakePlaceTime),
                            RecipientFullName = host.FullName,
                            Email = host.Email,
                            UserId = host.Id,
                        },
                        new SendEmail{
                            Subject = string.Format(SystemDataFormat.EXAM_NEWRELEVANCE_EMAIL_SUBJECT, "REVIEWER"),
                            Body = string.Format(SystemDataFormat.EXAM_NEWRELEVANCE_EMAIL_BODY,
                                            reviewer.UserName,
                                            requestedUser.UserName,
                                            "reviewer",
                                            newExam.ExamName,
                                            examType.ExamTypeName,
                                            newExam.ExamName,
                                            examType.Levels.LevelName,
                                            newExam.TakePlaceTime),
                            RecipientFullName = reviewer.FullName,
                            Email = reviewer.Email,
                            UserId = reviewer.Id,
                        }
                    ];
                    await Task.Run(() =>
                    {
                        // after 1 day auto-generate papers and send email to the relevances
                        // the next 3 lines is for testing
                        //_unitOfWork.Schedule<ExamPaperService>((x) =>
                        //         x.AutoGenPapersAndNotiToRelevances(newExam.Id),
                        //         currentTime.AddMinutes(3));
                        /*_unitOfWork.Schedule<ExamPaperService>((x) =>
                            x.AutoGenPapersAndNotiToRelevances(newExam.Id),
                            currentTime.AddDays(SystemDataFormat.EXAM_INFO_EDIT_DURATION));
                        // Auto upload pdf and audio file to azure blob storage
                        _unitOfWork.Schedule<ExamPaperService>((x) =>
                            x.UploadExamMaterials(newExam.Id, null, ""),
                            currentTime.AddDays(SystemDataFormat.EXAM_INFO_EDIT_DURATION + SystemDataFormat.EXAM_PAPERS_EDIT_DURATION));
                        // Auto suspend exam after 3 days if there is no papers generated
                        _unitOfWork.Schedule<ExamService>((x) =>
                            x.AutoSuspendExam(newExam.Id),
                            currentTime.AddDays(SystemDataFormat.EXAM_INFO_EDIT_DURATION + SystemDataFormat.EXAM_PAPERS_EDIT_DURATION));
                       */ // schedulte job auto lock all exam papers after 3 days and re-open 2 days before exam taking place
                        //_unitOfWork.Enqueue<EmailNotiService>((x) => x.SendEmails(emails));
                    });

                    response.IsSuccess = true;
                    response.Message = AppMessages.EXAM_CREATE_SUCCESS;
                    response.StatusCode = System.Net.HttpStatusCode.Created;
                    response.Result.Add(_mapper.Map<ExamComplexDisplayDTO>(newExam));
                }
                catch (Exception)
                {
                    response.StatusCode = System.Net.HttpStatusCode.Conflict;
                    response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                }
            }
            return response;
        }
        public async Task<APIResponse<object>> AutoSuspendExam(Guid examId)
        {
            APIResponse<object> response = new() { IsSuccess = false };
            Exam? exam = null;
            try
            {
                exam = await _unitOfWork.Exams.GetAsync(xx => xx.Id == examId, includeProperties: "ExamType,ExamType.ExamTypeConstraints,Host,Reviewer,CreatedUser")
                    ?? throw new VersionNotFoundException(AppMessages.TARGET_ITEM_NOTFOUND);
                if (exam.IsSuspended)
                {
                    response.Message = "The exam has been already suspended ~";
                    response.StatusCode = System.Net.HttpStatusCode.NoContent;
                    return response;
                }
                if (exam.Papers.Count == exam.ExamType.ExamTypeConstraints.Sum(x => x.NumberOfPaper))
                {
                    response.Message = "The exam has enough exam papers ~ then ok, the system did not perform auto suspend it";
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                await Console.Out.WriteLineAsync(ex.Message);
                return response;
            }
            try
            {
                DateTime currentTime = DateTime.Now;
                await _unitOfWork.BeginTransactionAsync();

                exam.IsSuspended = true;
                exam.UpdatedDate = currentTime;
                //await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                {
                    AccessTime = currentTime,
                    ActionName = "Auto suspend exam",
                    IpAddress = "",
                    IsSuccess = true,
                    TargetObject = $"{exam.ExamName}",
                    Details = "Auto suspend exam because no papers detected after editing time",
                    UserId = Guid.Empty,
                });
                await Task.Run(() =>
                {
                   // Send email to relevances that the exam has been automatically suspended because no papers was detected after editing time
                    List<SendEmail> emails =
                    [
                        new SendEmail {
                            Email = exam.Host.Email,
                            Body = $"The exam {exam.ExamName } has been automatically suspended because no papers detected after editing time",
                            RecipientFullName = exam.Host.UserName,
                            Subject = "AUTO SUSPEND EXAM",
                            UserId = exam.HostId
                        },
                        new SendEmail {
                            Email = exam.Reviewer.Email,
                            Body = $"The exam {exam.ExamName } has been automatically suspended because no papers detected after editing time",
                            RecipientFullName = exam.Reviewer.UserName,
                            Subject = "AUTO SUSPEND EXAM",
                            UserId = exam.ReviewerId
                        }
                    ];
                    _unitOfWork.Enqueue<EmailNotiService>((x) => x.SendEmails(emails));
                });

                await _unitOfWork.CommitAsync();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                await Console.Out.WriteLineAsync(ex.Message);
                return response;
            }
            return response;
        }
        public async Task<APIResponse<object>> DeleteExamAsync(Delete request)

        {
            APIResponse<object> response = new() { IsSuccess = false };
            try
            {
                var requestedUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.RequestedUserId,
                                                                     includeProperties: "Role");
                if (requestedUser == null || requestedUser.RoleId.ToString() != LogicString.Role.TeamLeadRoleId)
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.Message = AppMessages.NO_PERMISSION;
                    return response;
                }
                var targetExam = await _unitOfWork.Exams
                                .GetAsync(filter: x => x.Id == request.TargetObjectId,
                                          includeProperties: "ExamType,ExamType.Level,Papers,Host,Reviewer");
                if (targetExam == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                    return response;
                }
                if (targetExam.TakePlaceTime < DateTime.Now.AddDays(SystemDataFormat.DURATION_BEFORE_TAKEPLACETIME_CAN_DELETE))
                {
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.Message = AppMessages.CAN_ONLY_DELETE_EXAM_IN_LIMIT_TIME;
                    return response;
                }
                if (targetExam.Papers.Count != 0)
                {
                    response.StatusCode = System.Net.HttpStatusCode.Conflict;
                    response.Message = AppMessages.EXAM_DELETE_FAILED_CUZ_CONFLICT;
                    return response;
                }

                DateTime currentTime = DateTime.Now;
                if (request.HardDelete)
                {
                    var deleted = await _unitOfWork.Exams.DeleteExamAsync(request.TargetObjectId);
                    if (deleted)
                    {
                        await _unitOfWork.AccessLogs.AddAsync(new()
                        {
                            AccessTime = DateTime.Now,
                            ActionName = "Xóa",
                            IpAddress = request.IpAddress ?? "::1",
                            IsSuccess = true,
                            TargetObject = nameof(Exam),
                            UserId = request.RequestedUserId,
                            Details = $"Xóa kỳ thi {targetExam.ExamName}"
                        });
                        response.IsSuccess = true;
                        response.StatusCode = System.Net.HttpStatusCode.OK;
                        response.Message = AppMessages.EXAM_DELETE_SUCCESS;
                    }
                    else
                    {
                        response.IsSuccess = true;
                        response.Message = AppMessages.NO_CHANGES_DETECTED;
                        response.StatusCode = System.Net.HttpStatusCode.NoContent;
                    }
                }
                else
                {
                    response.Message = "Feature has not been implemented yet";
                    response.StatusCode = System.Net.HttpStatusCode.NotImplemented;
                    
                }
                response.Result.Add(_mapper.Map<ExamComplexDisplayDTO>(targetExam));
            }
            catch (Exception)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;
        }
        public async Task<APIResponse<object>> EditExamAsync(EditExamRequest request)

        {
            APIResponse<object> response = new() { IsSuccess = false };
            var requestedUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.RequestedUserId);
            if (requestedUser == null || requestedUser.RoleId.ToString() != LogicString.Role.TeamLeadRoleId)
            {
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.Message = AppMessages.NO_PERMISSION;
                return response;
            }
            var targetExam = await _unitOfWork.Exams
                            .GetAsync(filter: x => x.Id == request.TargetObjectId,
                                      includeProperties: "ExamType,ExamType.Levels,Papers,Host,Reviewer");
            if (targetExam == null)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                return response;
            }
            if (targetExam.Papers.Count != 0)
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Message = AppMessages.UPDATE_EXAM_HAS_PAPER;
                return response;
            }
            DateTime currentTime = DateTime.Now;
            if (targetExam.CreatedDate.AddDays(SystemDataFormat.EXAM_INFO_EDIT_DURATION) < currentTime)
            {
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Message = "Bạn chỉ có thể chỉnh sửa thông tin kỳ thi trong vòng 1 ngày sau khi tạo."; ;
                return response;
            }
            await _unitOfWork.BeginTransactionAsync();
            List<string> changes = [];
            DateTime earliestNewTime = currentTime.AddDays(SystemDataFormat.EARLIEST_EXAM_TAKEPLACETIME_FROMNOW);
            ExamType? examType = targetExam.ExamType;
            List<string> validateResult = [];
            User? oldHost = targetExam.Host;
            User? newHost = targetExam.Host;
            User? oldReviewer = targetExam.Reviewer;
            User? newReviewer = targetExam.Reviewer;
            List<SendEmail> needSendEmails = [];
            if (request.NewExamTypeId != null && examType.Id != request.NewExamTypeId)
            {
                examType = await _unitOfWork.ExamTypes.GetAsync(x => x.Id == request.NewExamTypeId,
                                                                includeProperties: "Level");
                if (examType != null)
                {
                    targetExam.ExamType = examType;
                    targetExam.ExamTypeId = (Guid)request.NewExamTypeId;
                    changes.Add("Thay đổi loại kỳ thi");
                }
                else validateResult.Add("Loại kỳ thi mới không tồn tại");
            }
            if (request.NewExamName != null)
            {
                if (!targetExam.ExamName.Trim().ToLower().Equals(request.NewExamName.Trim().ToLower()))
                {
                    var dupItem = await _unitOfWork.Exams.GetAsync(
                        filter: x => x.Id != targetExam.Id && x.ExamName.Trim().ToLower().Equals(request.NewExamName.Trim().ToLower()));
                    if (dupItem == null)
                    {
                        targetExam.ExamName = request.NewExamName;
                        changes.Add("Thay đổi tên kỳ thi");
                    }
                    else
                        validateResult.Add($"Đã tồn tại kỳ thi với tên: {request.NewExamName}");
                }
                else { }
            }
            if (request.NewTakePlaceTime != null && request.NewTakePlaceTime != targetExam.TakePlaceTime)
            {
                if (request.NewTakePlaceTime >= earliestNewTime)
                {
                    targetExam.TakePlaceTime = (DateTime)request.NewTakePlaceTime;
                    changes.Add("Thay đổi ngày thi");
                }
                else validateResult.Add("Sửa thời gian thi gấp quá, hong cho~");
            }
            if (request.NewHostId == request.NewReviewerId) validateResult.Add("Host và Reviewer phải là 2 người khác nhau để đúp bồ chếck");

            if (request.NewHostId != null && request.NewHostId != oldHost.Id)
            {
                newHost = await _unitOfWork.Users.GetAsync(x => x.Id == request.NewHostId);
                if (newHost != null && newHost.IsActive && newHost.RoleId.ToString() != LogicString.Role.AdminRoleId)
                {
                    targetExam.Host = newHost;
                    targetExam.HostId = newHost.Id;
                    // Handle new host
                    if (newHost.Id == oldReviewer.Id)
                    {
                        needSendEmails.Add(new()
                        {
                            UserId = newHost.Id,
                            Email = newHost.Email,
                            Subject = "ĐỔI VAI TRÒ TRONG KỲ THI",
                            Body = CommonUntils.PassParamsToFormat(
                                    SystemDataFormat.EXAM_CHANGERELEVANCE_EMAIL_BODY,
                                    newHost.UserName, "Reviewer", "Host", targetExam.ExamName),
                            RecipientFullName = newHost.FullName,
                        });
                        changes.Add("Đổi Reviewer thành Host");
                    }
                    else
                    {
                        needSendEmails.Add(new()
                        {
                            UserId = newHost.Id,
                            Email = newHost.Email,
                            Subject = "BẠN VỪA TRỞ THÀNH HOST CỦA MỘT KỲ THI",
                            Body = CommonUntils.PassParamsToFormat(SystemDataFormat.EXAM_NEWRELEVANCE_EMAIL_BODY,
                                    newHost.UserName, requestedUser.UserName, "Host", targetExam.ExamName,
                                    targetExam.ExamType.ExamTypeName, targetExam.ExamName,
                                    targetExam.ExamType.Levels.LevelName, targetExam.TakePlaceTime.ToString("dd/MM/yyyy")
                                    ),
                            RecipientFullName = newHost.FullName,
                        });
                        changes.Add("Đổi Host mới");
                    }
                    // Handle old host
                    if (oldHost.Id != newReviewer.Id)
                        needSendEmails.Add(new()
                        {
                            UserId = oldHost.Id,
                            Email = oldHost.Email,
                            Subject = "BẠN VỪA BỊ ĐÁ KHỎI KỲ THI",
                            Body = CommonUntils.PassParamsToFormat(SystemDataFormat.EXAM_KICKOUT_EMAIL_BODY,
                                    oldHost.UserName, requestedUser.UserName, targetExam.ExamName),
                            RecipientFullName = oldHost.FullName,
                        });
                    else
                        needSendEmails.Add(new()
                        {
                            UserId = oldHost.Id,
                            Email = oldHost.Email,
                            Subject = "ĐỔI VAI TRÒ TRONG KỲ THI",
                            Body = CommonUntils.PassParamsToFormat(
                                    SystemDataFormat.EXAM_CHANGERELEVANCE_EMAIL_BODY,
                                    oldHost.UserName, "Host", "Reviewer", targetExam.ExamName),
                            RecipientFullName = oldHost.FullName,
                        });
                }
                else validateResult.Add("Người dùng này không thể làm host được (his account has admin role / has been deleted or locked) ~");
            }
            else
            {
                if (changes.Count > 0) needSendEmails.Add(new()
                {
                    Email = oldHost.Email,
                    Subject = $"CẬP NHẬT KỲ THI {targetExam.ExamName}",
                    Body = $"Kỳ thi {targetExam.ExamName} mà bạn làm Host vừa được cập nhật",
                    RecipientFullName = oldHost.FullName,
                });
            }

            if (request.NewReviewerId != null && request.NewReviewerId != oldReviewer.Id)
            {
                newReviewer = await _unitOfWork.Users.GetAsync(x => x.Id == request.NewReviewerId);
                if (newReviewer != null && newReviewer.IsActive && newReviewer.RoleId.ToString() != LogicString.Role.AdminRoleId)
                {
                    targetExam.Reviewer = newReviewer;
                    targetExam.ReviewerId = newReviewer.Id;
                    // Handle new reviewer
                    if (newReviewer.Id != oldHost.Id)
                    {
                        needSendEmails.Add(new()
                        {
                            UserId = oldReviewer.Id,
                            Email = oldReviewer.Email,
                            Subject = "BẠN VỪA TRỞ THÀNH REVIEWER CỦA MỘT KỲ THI",
                            Body = CommonUntils.PassParamsToFormat(SystemDataFormat.EXAM_NEWRELEVANCE_EMAIL_BODY,
                                    oldReviewer.UserName, requestedUser.UserName, "Reviewer", targetExam.ExamName,
                                    targetExam.ExamType.ExamTypeName, targetExam.ExamName,
                                    targetExam.ExamType.Levels.LevelName, targetExam.TakePlaceTime.ToString("dd/MM/yyyy")
                                    ),
                            RecipientFullName = newReviewer.FullName,
                        });
                        changes.Add("Đổi Reviewer mới");
                    }
                    else
                    {
                        changes.Add("Đổi Host thành Reviewer");
                    }

                    // Handle old reviewer
                    if (oldReviewer.Id != newHost?.Id)
                        needSendEmails.Add(new()
                        {
                            UserId = oldReviewer.Id,
                            Email = oldReviewer.Email,
                            Subject = "BẠN VỪA BỊ ĐÁ KHỎI MỘT KỲ THI",
                            Body = CommonUntils.PassParamsToFormat(SystemDataFormat.EXAM_KICKOUT_EMAIL_BODY,
                                targetExam.Host.UserName, requestedUser.UserName, targetExam.ExamName),
                            RecipientFullName = oldReviewer.FullName,
                        });
                    else { }
                    changes.Add("Thay đổi exam reviewer");
                }
                else validateResult.Add("Người dùng này không thể làm reviewer được (his account has admin role / has been deleted or locked) ~");
            }
            else
            {
                if (changes.Count > 0) needSendEmails.Add(new()
                {
                    Email = oldReviewer.Email,
                    Subject = $"CẬP NHẬT KỲ THI {targetExam.ExamName}",
                    Body = $"Kỳ thi {targetExam.ExamName} mà bạn làm Reviewer vừa được cập nhật",
                    RecipientFullName = oldReviewer.FullName,
                });
            }

            if (changes.Count > 0)
            {
                if (validateResult.Count > 0)
                {
                    response.Message = "Thông tin kỳ thi mới bị bất ổn nha ~";
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    foreach (var item in validateResult)
                    {
                        response.Result.Add(item);
                    }
                }
                else
                {
                    try
                    {
                        await _unitOfWork.Exams.UpdateWithNoCommitAsync(targetExam);
                        
                        //await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.AccessLogs.AddAsync(new()
                        {
                            AccessTime = currentTime,
                            ActionName = "Chỉnh sửa kỳ thi",
                            IpAddress = request.IpAddress ?? "::1",
                            IsSuccess = true,
                            TargetObject = nameof(Exam),
                            UserId = request.RequestedUserId,
                            Details = string.Join(", ", changes)
                        });
                        //await Task.Run(() =>
                        //{
                        //    _unitOfWork.Enqueue<EmailNotiService>((x) => x.SendEmails(needSendEmails));
                        //});
                        await Console.Out.WriteLineAsync("Edited");
                        await _unitOfWork.CommitAsync();
                    }

                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackAsync();
                        await Console.Out.WriteLineAsync(ex.Message);
                        response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                        response.StatusCode = System.Net.HttpStatusCode.Conflict;
                    }
                    response.IsSuccess = true;
                    //response.Message = string.Join(".", changed);
                    response.Message = AppMessages.EXAM_UPDATE_SUCCESS;
                    response.Result.Add(_mapper.Map<ExamComplexDisplayDTO>(targetExam));
                }
            }
            else
            {
                response.Message = "Không có thay đổi";
                response.StatusCode = System.Net.HttpStatusCode.NoContent;
                response.IsSuccess = true;
            }
            return response;
        }

        public async Task<APIResponse<ExamAsTaskDisplayDTO>> GetExamAsTask(GetExamAsTaskRequest request)

        {
            APIResponse<ExamAsTaskDisplayDTO> response = new();

            Expression<Func<Exam, bool>> filter = x => true;
            if (request.UserId != null)
            {
                filter = ExpressionExtension.CombineFilters(filter, x => x.HostId == request.UserId || x.ReviewerId == request.UserId);
            }
            if (request.LevelId != null)
            {
                filter = ExpressionExtension.CombineFilters(filter, x => x.ExamType.LevelId == request.LevelId);
            }
            if (request.Occured == true)
            {
                filter = ExpressionExtension.CombineFilters(filter, x => x.TakePlaceTime >= DateTime.Now);
            }
            else if (request.Occured == false)
            {
                filter = ExpressionExtension.CombineFilters(filter, x => x.TakePlaceTime < DateTime.Now);
            }

            try
            {
                var total = await _unitOfWork.Exams.CountAsync(filter);
                var queriedResult = await _unitOfWork.Exams.GetAllAsync(filter, includeProperties: "ExamType,Host,Reviewer,ExamType.Levels", pageNumber: request.PaginationRequest.Page,
                    pageSize: request.PaginationRequest.Size);

                List<ExamAsTaskDisplayDTO> subList = queriedResult.Where(x => !request.UserId.HasValue || x.HostId == request.UserId)
                    .Select(x => new ExamAsTaskDisplayDTO

                {
                    ExamId = x.Id,
                    ExamName = x.ExamName,
                    LevelName = x.ExamType.Levels.LevelName,
                    ExamType = x.ExamType.ExamTypeName,
                    Occured = x.TakePlaceTime > DateTime.Now,
                    TakePlaceTime = x.TakePlaceTime,
                    TaskName = "Host",
                    UserName = x.Host.UserName
                }).ToList();
                List<ExamAsTaskDisplayDTO> fullList = queriedResult.Where(x => !request.UserId.HasValue || x.ReviewerId == request.UserId)
                     .Where(x => !subList.Any(h => h.ExamId == x.Id))
                    .Select(x => new ExamAsTaskDisplayDTO
                {
                    ExamId = x.Id,
                    ExamName = x.ExamName,
                    LevelName = x.ExamType.Levels.LevelName,
                    ExamType = x.ExamType.ExamTypeName,
                    Occured = x.TakePlaceTime < DateTime.Now,
                    TakePlaceTime = x.TakePlaceTime,
                    TaskName = "Review",
                    UserName = x.Reviewer.UserName
                }).ToList();
                
                List<ExamAsTaskDisplayDTO> finalList = request.Host switch
                {
                    true => subList,             // Chỉ lấy host
                    false => fullList,          // Chỉ lấy reviewer
                    null => subList.Concat(fullList).ToList() // Cả hai
                };

                // Trả kết quả
                if (!finalList.Any())
                {
                    response.StatusCode = System.Net.HttpStatusCode.NoContent;
                    response.Message = AppMessages.NO_CONTENT;
                }
                else
                {
                    response.Result = finalList.OrderByDescending(x => x.TakePlaceTime).ToList();
                    response.Message = $"{finalList.Count}";
                    response.TotalCount = total;
                    response.Pagination = new Pagination
                    {
                        Page = request.PaginationRequest.Page,
                        Size = request.PaginationRequest.Size
                    };
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
         

        public async Task<APIResponse<object>> GetExamAsync(Guid? id)

        {
            APIResponse<object> response = new();
            try
            {
                bool getAll = true;
                Expression<Func<Exam, bool>> filter = x => x.IsDeleted == false;
                if (id != null)
                {
                    getAll = false;
                    Expression<Func<Exam, bool>> additionFilter = x => x.Id == id;
                    filter = ExpressionExtension.CombineFilters(filter, additionFilter);
                }
                var queriedResult = await _unitOfWork.Exams.GetAllAsync(
                            filter: filter,
                            includeProperties: "ExamType,ExamType.Levels,Host,Reviewer",
                            orderBy: x => x.OrderByDescending(x => x.CreatedDate));
                var count = queriedResult.Count;
                if (getAll)
                {
                    if (count == 0)
                    {
                        response.StatusCode = System.Net.HttpStatusCode.NoContent;
                        response.Message = AppMessages.NO_CONTENT;
                    }
                    else
                    {
                        response.Message = $"~ {count} ~";
                        foreach (var item in queriedResult)
                        {
                            var mappedItem = _mapper.Map<ExamGeneralDisplayDTO>(item);
                            response.Result.Add(mappedItem);
                        }
                    }
                }
                else
                {
                    if (count == 1)
                    {
                        var result = queriedResult.First();
                        response.Result.Add(_mapper.Map<ExamComplexDisplayDTO>(result));
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.StatusCode = System.Net.HttpStatusCode.NotFound;
                        response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                    }
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

       
        public Task<APIResponse<object>> HideExam(Guid? requestedUserId, Guid examId, string? ipAddress = "::1", string keyword = "")
        {
            throw new NotImplementedException();
        }

        public Task<APIResponse<object>> UnhideExam(Guid? requestedUserId, Guid examId, string? ipAddress = "::1", string keyword = "")
        {
            throw new NotImplementedException();
        }

        public Task<APIResponse<object>> UserSuspendExam(Guid requestedUserId, Guid examId, string? reason, string ipAddress = "")
        {
            throw new NotImplementedException();
        }
    }
}
