using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using KEB.Application.DTOs.AnswerDTO;
using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.GeminiDto;
using KEB.Application.DTOs.QuestionAddDTO;
using KEB.Application.DTOs.QuestionDTO;
using KEB.Application.DTOs.UserDTO;
using KEB.Application.Services.Interfaces;
using KEB.Application.Utils;
using KEB.Domain.Entities;
using KEB.Domain.Enums;
using KEB.Domain.ValueObject;
using KEB.Infrastructure.UnitOfWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static KEB.Domain.ValueObject.LogicString;

namespace KEB.Application.Services.Implementations
{
    public class QuestionService : IQuestionService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;
        //private readonly string _apiKey;
        private readonly string _modelName;
        private readonly string _baseApiUrl;
        public QuestionService(IUnitOfWork unitOfWork, IMapper mapper, HttpClient httpClient, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpClient = httpClient;

            //_apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini:ApiKey not found");
            //_modelName = configuration["Gemini:ModelName"] ?? "gemini-1.5-flash";
            //_baseApiUrl = configuration["Gemini:BaseApiUrl"] ?? "https://generativelanguage.googleapis.com/v1beta/models/";

        }
     
        public async Task<APIResponse<QuestionDetailDto>> GetQuestionDetailAsync(Guid questionId)
        {
            APIResponse<QuestionDetailDto> response = new();
            try
            {
                var queriedQuestion = await _unitOfWork.Questions
                    .GetAsync(x => x.Id == questionId, "QuestionType,Answers," +
                                                       "LevelDetail,References,LevelDetail.Level,LevelDetail.Topic");
                if (queriedQuestion != null)
                {
                    var mappedQuestion = _mapper.Map<QuestionDetailDto>(queriedQuestion);

                    response.Result.Add(mappedQuestion);
                    response.IsSuccess = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                }
                else
                {
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                }
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;
        }

        public async Task<APIResponse<QuestionDisplayDto>> GetQuestionsListAsync(GetQuestionsRequest request)
        {
            Expression<Func<Question, bool>> filter = (x => true);
            if (request.CreatedBy != null)
            {
                Expression<Func<Question, bool>> creatorFilter = (x => x.CreatedBy == request.CreatedBy);
                filter = ExpressionExtension.CombineFilters(filter, creatorFilter);
            }
            if (request.LevelIds != null && request.LevelIds.Count > 0)
            {
                Expression<Func<Question, bool>> levelFilter = (x => request.LevelIds.Contains(x.LevelDetail.LevelId));
                filter = ExpressionExtension.CombineFilters(filter, levelFilter);
            }
            if (request.TopicIds != null && request.TopicIds.Count > 0)
            {
                Expression<Func<Question, bool>> topicFilter = (x => request.TopicIds.Contains(x.LevelDetail.TopicId));
                filter = ExpressionExtension.CombineFilters(filter, topicFilter);
            }
            if (request.ReferenceIds != null && request.ReferenceIds.Count != 0)
            {
                Expression<Func<Question, bool>> referenceFilter = (x => request.ReferenceIds.Contains(x.ReferenceId));
                filter = ExpressionExtension.CombineFilters(filter, referenceFilter);
            }
            if (request.QuestionTypeIds != null && request.QuestionTypeIds.Count > 0)
            {
                Expression<Func<Question, bool>> quesTypeFilter = (x => request.QuestionTypeIds.Contains(x.QuestionTypeId));
                filter = ExpressionExtension.CombineFilters(filter, quesTypeFilter);
            }
            if (request.Skill != null)
            {
                Expression<Func<Question, bool>> skillFilter = (x => x.QuestionType.Skill == request.Skill);
                filter = ExpressionExtension.CombineFilters(filter, skillFilter);
            }
            if (request.Difficulties != null && request.Difficulties.Count > 0)
            {
                Expression<Func<Question, bool>> difficultyFilter = (x => request.Difficulties.Contains(x.Difficulty));
                filter = ExpressionExtension.CombineFilters(filter, difficultyFilter);
            }
            if (request.Status != null && request.Status.Count > 0)
            {
                Expression<Func<Question, bool>> quesStatusFilter = (x => request.Status.Contains(x.Status));
                filter = ExpressionExtension.CombineFilters(filter, quesStatusFilter);
            }
            if (!string.IsNullOrEmpty(request.SearchContent))
            {
                Expression<Func<Question, bool>> quesContentFilter = x => x.QuestionContent.Contains(request.SearchContent.ToLower());
                filter = ExpressionExtension.CombineFilters(filter, quesContentFilter);
            }
            if (request.IsMultipleChoice != null)
            {
                Expression<Func<Question, bool>> questionFormFilter = (x => x.IsMultipleChoice == request.IsMultipleChoice);
                filter = ExpressionExtension.CombineFilters(filter, questionFormFilter);
            }
            if (request.LogId != null)
            {
                Expression<Func<Question, bool>> logFilter = (x => x.LogId == request.LogId);
                filter = ExpressionExtension.CombineFilters(filter, logFilter);
            }
            if (request.TaskId != null)
            {
                Expression<Func<Question, bool>> taskFilter = (x => x.TaskId == request.TaskId);
                filter = ExpressionExtension.CombineFilters(filter, taskFilter);
            }
            if (request.FromTime != null && request.ToTime != null)
            {
                Expression<Func<Question, bool>> timeFilter = x => x.CreatedDate >= request.FromTime && x.CreatedDate <= request.ToTime;
                filter = ExpressionExtension.CombineFilters(filter, timeFilter);
            }
            var total = await _unitOfWork.Questions.CountAsync(filter);
            var queryResult = await _unitOfWork.Questions.GetAllAsync(
                    filter: filter,
                    includeProperties: "QuestionType,Answers,LevelDetail,References,LevelDetail.Level,LevelDetail.Topic,AttachmentFileImage,AttachmentFileAudio ",
                    pageNumber: request.PaginationRequest?.Page ?? 0,
                    pageSize: request.PaginationRequest?.Size ?? 0,
                    orderBy: x =>
                    {
                        if (request.SortAscending) return x.OrderBy(x => x.CreatedDate);
                        else return x.OrderByDescending(x => x.CreatedDate);
                    });
            var mappedData = _mapper.Map<List<QuestionDisplayDto>>(queryResult);

            var response = new APIResponse<QuestionDisplayDto>
            {
                Result = mappedData,
                Message = "Lấy danh sách câu hỏi thành công",
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                TotalCount = total,
                Pagination = new Pagination
                {
                    Page = request.PaginationRequest.Page,
                    Size = request.PaginationRequest.Size
                }
            };

            return response;

        }

        public async Task<APIResponse<ChangeStatusResultDTO>> TeamLeadChangeQuestionStatus(ChangeQuestionStatusRequest request)
        {
            APIResponse<ChangeStatusResultDTO> response = new()
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest
            };
            try
            {
                var requestedUser = await _unitOfWork.Users.GetUserById(request.RequestedUserId);

                if (requestedUser == null || requestedUser.RoleId.ToString() != LogicString.Role.TeamLeadRoleId)
                {
                    response.StatusCode = HttpStatusCode.Forbidden;
                    response.Message = AppMessages.NO_PERMISSION;
                    return response;
                }
                List<string> validError = [];
                var importHistory = await _unitOfWork.AccessLogs.GetAsync(x => x.Id == request.LogId && x.TargetObject == nameof(Question),
                                                                includeProperties: "User");
                request.NotifyTo = importHistory.UserId;
                if (importHistory == null)
                {
                    validError.Add("Import history did not exist");
                }
                else
                {
                    if (importHistory.UserId != request.NotifyTo)
                        validError.Add("It was a dumb request when notifying to an user that is irrelevant to this import history");
                }
                // Do not accept empty request list
                if (request.Requests.Count == 0)
                {
                    validError.Add("No requests received ~");
                }
                // Do not accept more than 50 requests on one time
                if (request.Requests.Count > 50)
                {
                    validError.Add("Only allow to make changes on maximum of 50 questions in 1 request ~");
                }
                if (!request.Requests.All(x => x.ToStatus == QuestionStatus.Ok || x.ToStatus == QuestionStatus.Denied))
                {
                    validError.Add("Teamm lead is only authorized to deny or approve questions as OK ~");
                }
                // Deny question request must be attached with a reason
                var lackOfReason = request.Requests.Where(x => x.ToStatus == QuestionStatus.Denied && string.IsNullOrEmpty(x.Reason));
                if (lackOfReason.Any())
                {
                    validError.Add("A reasonable cause must be attached to deny a question' ~");
                }
                if (validError.Count != 0)
                {
                    response.Message = string.Join("<br/>", validError);
                    return response;
                }
                // Order the requests list
                var orderedRequests = request.Requests.OrderBy(x => x.QuestionId);
                var idList = request.Requests.Select(x => x.QuestionId).OrderBy(x => x);
                // Get questions from database
                var targetQuestions = (await _unitOfWork.Questions.GetAllAsync(filter: x => idList.Contains(x.Id),
                                                orderBy: x => x.OrderBy(x => x.Id), asTracking: true,
                                                includeProperties: "Answers"))
                                                .ToList();
                // All questions to be processed must exist in the system
                if (targetQuestions.Count < idList.Count())
                {
                    validError.Add("Some questions can not be found to be processed");
                }
                //if (targetQuestions.Any(x => x.LogId != request.LogId))
                //{
                //    validError.Add("Can only approve/deny questions in the same import history");
                //}
                // Can only modified questions of pending status
                if (targetQuestions.Any(x => x.Status != QuestionStatus.Pending))
                {
                    validError.Add(AppMessages.ONLY_PENDING_QUESTION_CAN_BE_EDITED);
                }
                if (validError.Count != 0)
                {
                    response.Message = string.Join("<br/>", validError);
                    return response;
                }
                // Begin transaction
                List<SystemAccessLog> logs = [];
                List<ChangeStatusResultDTO> result = [];
                DateTime currentTime = DateTime.Now;
                await _unitOfWork.BeginTransactionAsync();
                int requestIndex = 0;
                int approved = 0;
                int denied = 0;
                foreach (var item in orderedRequests)
                {
                    logs.Add(new SystemAccessLog
                    {
                        AccessTime = currentTime,
                        ActionName = $"Change question status",
                        Details = $"Change question status from {targetQuestions[requestIndex].Status} to {item.ToStatus}",
                        IpAddress = request.IpAddress ?? "::1",
                        IsSuccess = true,
                        TargetObject = nameof(Question),
                        UserId = request.RequestedUserId,
                    });
                    result.Add(new ChangeStatusResultDTO
                    {
                        QuestionId = targetQuestions[requestIndex].Id,
                        QuestionContent = targetQuestions[requestIndex].ToString(),
                        Action = $"Change question status from {targetQuestions[requestIndex].Status} to {item.ToStatus}"
                    });

                    targetQuestions[requestIndex].Status = item.ToStatus;
                    targetQuestions[requestIndex].Description = item.Reason;
                    targetQuestions[requestIndex].UpdatedDate = currentTime;
                    targetQuestions[requestIndex].UpdatedBy = request.RequestedUserId;
                    requestIndex++;
                    if (item.ToStatus == QuestionStatus.Denied) denied++;
                    else approved++;
                }
                // Log changes
                await _unitOfWork.AccessLogs.AddRangeAsync(logs);
                // Save changes and commit
                await _unitOfWork.SaveChangesAsync(); // AddRangeAsync func has already called save changes

                // New notification
                await _unitOfWork.Notifications.AddAsync(new Notification
                {
                    UserId = request.NotifyTo,
                    CreatedDate = currentTime,
                    Description = $"Quản lý {requestedUser.UserName} " +
                                  $"đã duyệt {approved} câu hỏi và từ chối {denied} câu hỏi" +
                                  $" lý do là {request.Requests[0].Reason}",
                });

                // Send email to the created user
                await Task.Run(() =>
                {
                    _unitOfWork.EmailService.SendEmail(importHistory.User.Email,
                                                "QUẢN LÝ ĐÃ THAY ĐỔI TRẠNG THÁI CÂU HỎI",
                                                $"Quản lý {requestedUser.UserName} " +
                                                        $"đã duyệt {approved} câu hỏi và từ chối {denied} câu hỏi trong {importHistory.ActionName: dd MMM yyyy} ",
                                                importHistory.User.UserName);
                });
                response.Message = $"{approved};{denied}";
                response.StatusCode = System.Net.HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = result;
                await _unitOfWork.CommitAsync();
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.Message);
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;
        }

        public async Task<APIResponse<QuestionDetailDto>> TeamLeadDeleteQuestion(Delete request)
        {
            APIResponse<QuestionDetailDto> response = new()
            {
                IsSuccess = false,
            };
            try
            {
                var targetQuestion = await _unitOfWork.Questions
                    .GetAsync(x => x.Id == request.TargetObjectId, "QuestionType,Answers," +
                                                                "CreatedUser,UpdatedUser");
                if (targetQuestion == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                    return response;
                }
                var user = await _unitOfWork.Users.GetUserById(request.RequestedUserId);
                if (user == null || user.RoleId.ToString() != LogicString.Role.TeamLeadRoleId)
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.Message = AppMessages.NO_PERMISSION;
                    return response;
                }
                var deleteResult = await _unitOfWork.Questions.DeleteQuestionAsync(targetQuestion);
                await _unitOfWork.AccessLogs.AddAsync(new()
                {
                    TargetObject = AccessLogConstant.QUESTIONS_CONTROLLER,
                    ActionName = string.Format(AccessLogConstant.DELETE_ACTION, "1 câu hỏi"),
                    IpAddress = request.IpAddress ?? "",
                    IsSuccess = false,
                    UserId = request.RequestedUserId,
                    AccessTime = DateTime.Now,
                    Details = $"{user.UserName} xóa 1 câu hỏi và {deleteResult.Answers} câu trả lời tương ứng!"
                });
                var mappedQuestion = _mapper.Map<QuestionDetailDto>(targetQuestion);
                response.IsSuccess = true;
                response.Message = AppMessages.QUESTION_DELETE_SUCCESS;
                response.Result.Add(mappedQuestion);
            }
            catch (ArgumentNullException e)
            {
                response.Message = e.Message;
            }
            catch (Exception)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;
        }

        public async Task<APIResponse<object>> UpdateQuestion(UpdateQuestionRequest request)
        {
            var response = new APIResponse<object> { IsSuccess = false };
            try
            {
                var question = await _unitOfWork.Questions.GetAsync(
                    x => x.Id == request.TargetObjectId,
                    asTracking: true,
                    includeProperties: "QuestionType,Answers,LevelDetail,References,LevelDetail.Level,LevelDetail.Topic")
                    ;

                if (question == null)
                {
                    response.Message = "Không tìm thấy câu hỏi";
                    response.StatusCode = HttpStatusCode.NotFound;
                    return response;
                }
                if (request.RequestedUserId != question.CreatedBy)
                {
                    response.Message = "Không có quyền chỉnh sửa câu hỏi này";
                    response.StatusCode = HttpStatusCode.Forbidden;
                    return response;
                }

                if (question.Status == QuestionStatus.Ok)
                {
                    response.Message = "Chỉ có thể sửa câu hỏi ở trạng thái 'Chờ duyệt'";
                    response.StatusCode = HttpStatusCode.Forbidden;
                    return response;
                }

                await _unitOfWork.BeginTransactionAsync();
                var changes = new List<string>();
                var now = DateTime.Now;

                // Nội dung
                if (!string.IsNullOrEmpty(request.NewQuestionContent) && request.NewQuestionContent != question.QuestionContent)
                {
                    question.QuestionContent = request.NewQuestionContent;
                    changes.Add("Đổi nội dung câu hỏi");
                }

                // Độ khó
                if (request.NewDifficulty != null && request.NewDifficulty != question.Difficulty)
                {
                    question.Difficulty = (Difficulty)request.NewDifficulty;
                    changes.Add("Đổi độ khó");
                }

                // Reference
                if (request.NewReferenceId != null && request.NewReferenceId != question.ReferenceId)
                {
                    var newRef = await _unitOfWork.References.GetAsync(x => x.Id == request.NewReferenceId);
                    if (newRef == null)
                    {
                        response.Message = "Không tìm thấy tài liệu tham khảo";
                        response.StatusCode = HttpStatusCode.BadRequest;
                        return response;
                    }

                    question.ReferenceId = newRef.Id;
                    changes.Add("Đổi tài liệu tham khảo");
                }

                // Xử lý Answers
                if (request.AnswersChanged && request.Answers != null && request.Answers.Any())
                {
                    // Xóa các câu trả lời cũ
                    var oldAnswers = question.Answers.ToList();
                    foreach (var oldAnswer in oldAnswers)
                    {
                        _unitOfWork.Answers.RemoveAsync(oldAnswer);
                    }

                    // Thêm các câu trả lời mới
                    foreach (var answerDto in request.Answers)
                    {
                        var newAnswer = _mapper.Map<Answer>(answerDto);
                        newAnswer.QuestionId = question.Id;
                        newAnswer.CreatedBy = request.RequestedUserId;
                        newAnswer.CreatedDate = now;
                        await _unitOfWork.Answers.AddAsync(newAnswer);
                    }
                    changes.Add("Cập nhật danh sách câu trả lời");
                }

                // Xử lý Attachments (Image và Audio)
                if (request.AttachmentChanged)
                {
                    // Xử lý Image Attachment
                    if (request.AttachmentFileImage != null)
                    {
                        // Xóa file ảnh cũ nếu có
                        if (question.AttachFileImageId.HasValue)
                        {
                            var oldImage = await _unitOfWork.ImageFiles.GetAsync(x => x.Id == question.AttachFileImageId);
                            if (oldImage != null)
                            {
                                _unitOfWork.ImageFiles.RemoveAsync(oldImage);
                            }
                        }

                        // Thêm file ảnh mới
                        var newImage = await GetAttachFile.GetImageFile(request.AttachmentFileImage);
                        await _unitOfWork.ImageFiles.AddAsync(newImage);
                        question.AttachFileImageId = newImage.Id;
                        changes.Add("Cập nhật tệp ảnh");
                    }
                    else if (request.AttachmentFileImage == null && question.AttachFileImageId.HasValue)
                    {
                        // Xóa file ảnh nếu AttachmentChanged = true và không có file mới
                        var oldImage = await _unitOfWork.ImageFiles.GetAsync(x => x.Id == question.AttachFileImageId);
                        //if (oldImage != null)
                        //{
                        //    _unitOfWork.ImageFiles.Remove(oldImage);
                        //}
                        question.AttachFileImageId = null;
                        changes.Add("Xóa tệp ảnh");
                    }

                    // Xử lý Audio Attachment
                    if (request.AttachmentFileAudio != null)
                    {
                        // Xóa file audio cũ nếu có
                        if (question.AttachFileAudioId.HasValue)
                        {
                            var oldAudio = await _unitOfWork.ImageFiles.GetAsync(x => x.Id == question.AttachFileAudioId);
                            if (oldAudio != null)
                            {
                                _unitOfWork.ImageFiles.RemoveAsync(oldAudio);
                            }
                        }

                        // Thêm file audio mới
                        var newAudio = await GetAttachFile.GetImageFile(request.AttachmentFileAudio);
                        await _unitOfWork.ImageFiles.AddAsync(newAudio);
                        question.AttachFileAudioId = newAudio.Id;
                        changes.Add("Cập nhật tệp âm thanh");
                    }
                    else if (request.AttachmentFileAudio == null && question.AttachFileAudioId.HasValue)
                    {
                        // Xóa file audio nếu AttachmentChanged = true và không có file mới
                        var oldAudio = await _unitOfWork.ImageFiles.GetAsync(x => x.Id == question.AttachFileAudioId);
                        if (oldAudio != null)
                        {
                            _unitOfWork.ImageFiles.RemoveAsync(oldAudio);
                        }
                        question.AttachFileAudioId = null;
                        changes.Add("Xóa tệp âm thanh");
                    }
                }

                if (!changes.Any())
                {
                    response.IsSuccess = true;
                    response.Message = "Không có gì thay đổi";
                    response.StatusCode = HttpStatusCode.NoContent;
                    return response;
                }

                question.UpdatedDate = now;
                question.UpdatedBy = request.RequestedUserId;
                question.Status = QuestionStatus.Pending;
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                {
                    AccessTime = now,
                    ActionName = "Chỉnh sửa câu hỏi",
                    Details = string.Join(" ~ ", changes),
                    IpAddress = request.IpAddress ?? "::1",
                    IsSuccess = true,
                    TargetObject = nameof(Question),
                    UserId = request.RequestedUserId
                });
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.Map<QuestionDetailDto>(question);
                await _unitOfWork.CommitAsync();

                response.Result.Add(result);
                response.IsSuccess = true;
                response.Message = "Cập nhật thành công";
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = $"Có lỗi xảy ra khi cập nhật câu hỏi: {ex.Message}";
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }

        public async Task<APIResponse<ImportQuestionResultDTO>> UploadQuestionFromExcel(ImportQuestionFromExcelRequest request, string ipAddress)
        {
            var response = new APIResponse<ImportQuestionResultDTO> { IsSuccess = false };
            var result = new ImportQuestionResultDTO
            {
                Question = new List<QuestionDetailDto>(),
                Messages = new List<string>()
            };

            try
            {
                if (request.ExcelFile == null || request.ExcelFile.Length == 0)
                {
                    response.Message = "Vui lòng chọn file Excel";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Result = new List<ImportQuestionResultDTO> { result };
                    return response;
                }
                var excelExtension = Path.GetExtension(request.ExcelFile.FileName).ToLower();
                if (excelExtension != ".xlsx")
                {
                    response.Message = "Chỉ hỗ trợ file Excel (.xlsx)";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Result = new List<ImportQuestionResultDTO> { result };
                    return response;
                }
                var attachmentDict = new Dictionary<string, IFormFile>(StringComparer.OrdinalIgnoreCase);
                foreach (var file in request.Attachments ?? Enumerable.Empty<IFormFile>())
                {
                    if (file != null && file.Length > 0)
                    {
                        attachmentDict[file.FileName] = file;
                    }
                }
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var stream = request.ExcelFile.OpenReadStream();
                using var excel = new ExcelPackage(stream);
                await _unitOfWork.BeginTransactionAsync();
                var changes = new List<string>();
                var now = DateTime.Now;
                var sheet = request.ForMultipleChoice ? excel.Workbook.Worksheets["TRẮC NGHIỆM"] : excel.Workbook.Worksheets["TỰ LUẬN"];
                if (sheet == null)
                {
                    response.Message = $"Không tìm thấy sheet {(request.ForMultipleChoice ? "TRẮC NGHIỆM" : "TỰ LUẬN")} trong file Excel";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    result.Messages.Add(response.Message);
                    await _unitOfWork.RollbackAsync();
                    response.Result = new List<ImportQuestionResultDTO> { result };
                    return response;
                }
                for (int row = 3; row <= sheet.Dimension.End.Row; row++)
                {
                    if (string.IsNullOrWhiteSpace(sheet.Cells[row, 5].Text)) continue;
                    AddSingleQuestionRequest questionRequest;
                    if (request.ForMultipleChoice)
                    {
                        questionRequest = await ParseMultipleChoiceRow(sheet, row, request.RequestedUserId, request.TaskId.Value, attachmentDict);
                    }
                    else
                    {
                        questionRequest = await ParseEssayRow(sheet, row, request.RequestedUserId, attachmentDict);
                    }
                    if (questionRequest == null)
                    {
                        result.Messages.Add($"Hàng {row}: Dữ liệu không hợp lệ hoặc tệp đính kèm không khớp");
                        continue;
                    }
                    questionRequest.TaskId = request.TaskId;
                    var addResult = await AddSingleQuestionAsync(questionRequest);
                    if (addResult.IsSuccess && addResult.Result.Any())
                    {
                        ((List<QuestionDetailDto>)result.Question).AddRange(addResult.Result);
                        changes.Add($"Thêm câu hỏi {(request.ForMultipleChoice ? "trắc nghiệm" : "tự luận")}: {questionRequest.QuestionContent}");
                    }
                    else
                    {
                        result.Messages.Add($"Hàng {row}: {addResult.Message}");
                    }
                }
                if (!changes.Any())
                {
                    response.Message = "Không có câu hỏi nào được thêm từ file Excel";
                    response.StatusCode = HttpStatusCode.NoContent;
                    result.Messages.Add(response.Message);
                    await _unitOfWork.RollbackAsync();
                    response.Result = new List<ImportQuestionResultDTO> { result };
                    return response;
                }
                await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                {
                    AccessTime = now,
                    ActionName = "Thêm câu hỏi từ Excel",
                    Details = string.Join(" ~ ", changes),
                    IpAddress = ipAddress ?? "::1",
                    IsSuccess = true,
                    TargetObject = nameof(Question),
                    UserId = request.RequestedUserId
                });
                await _unitOfWork.CommitAsync();
                if (request.TaskId.HasValue)
                {
                    var task = await _unitOfWork.AddQuestionTasks.GetByIdAsync(request.TaskId.Value);
                    if (task != null)
                    {
                        var actor = await _unitOfWork.Users.GetByIdAsync(request.RequestedUserId);
                        var taskCreator = await _unitOfWork.Users.GetByIdAsync(task.CreatedBy.Value);
                        if (taskCreator != null && !string.IsNullOrEmpty(taskCreator.Email))
                        {
                            try
                            {
                                _unitOfWork.EmailService.SendEmail(
                                    taskCreator.Email,
                                    "HỆ THỐNG CÓ CÂU HỎI MỚI ĐƯỢC THÊM VÀO TASK CỦA BẠN",
                                    $"{actor?.UserName} vừa thêm {((List<QuestionDetailDto>)result.Question).Count} câu hỏi mới từ file Excel vào tác vụ bạn đã tạo lúc {now:dd/MM/yyyy HH:mm}. " +
                                    "Vui lòng truy cập hệ thống để kiểm tra và duyệt câu hỏi.",
                                    taskCreator.FullName
                                );
                            }
                            catch (Exception ex)
                            {
                                result.Messages.Add("Thêm câu hỏi thành công nhưng không thể gửi email thông báo.");
                            }
                        }
                    }
                }
                response.Result = new List<ImportQuestionResultDTO> { result };
                response.IsSuccess = true;
                response.Message = $"Thêm {((List<QuestionDetailDto>)result.Question).Count} câu hỏi từ Excel thành công";
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = $"Lỗi khi thêm câu hỏi từ Excel: {ex.Message}";
                response.StatusCode = HttpStatusCode.InternalServerError;
                result.Messages.Add(response.Message);
                response.Result = new List<ImportQuestionResultDTO> { result };
            }
            return response;
        }
        private async Task<AddSingleQuestionRequest> ParseMultipleChoiceRow(ExcelWorksheet sheet, int row, Guid requestedUserId, Guid taskId, Dictionary<string, IFormFile> attachments)
        {
            var request = new AddSingleQuestionRequest
            {
                RequestedUserId = requestedUserId,
                TaskId = taskId,
                IsMultipleChoice = true,
                Answers = new List<AddAnswerDTO>()
            };

            // Parse Nguồn
            var refText = sheet.Cells[row, 1].Text;
            if (!string.IsNullOrEmpty(refText))
            {
                var refParts = refText.Split('-');
                var refName = refParts[0];
                var refs = await _unitOfWork.References.GetAsync(x => x.ReferenceName == refName);
                if (refs != null)
                    request.ReferenceId = refs.Id;
                else
                    return null;
            }

            // Parse Chủ đề
            var levelText = sheet.Cells[row, 2].Text;
            if (!string.IsNullOrEmpty(levelText))
            {
                var levelParts = levelText.Split('-');
                var levelName = levelParts[0];
                var topicName = levelParts.Length > 1 ? levelParts[1] : "";
                var levelDetail = await _unitOfWork.LevelDetails.GetAsync(x => x.Level.LevelName == levelName && x.Topic.TopicName == topicName);
                if (levelDetail != null)
                    request.LevelDetailId = levelDetail.Id;
                else
                    return null;
            }

            // Parse Loại câu hỏi
            var typeText = sheet.Cells[row, 3].Text;
            if (!string.IsNullOrEmpty(typeText))
            {

                var typeParts = typeText.Split('-');
                var skill = typeParts[0];
                Skill skillEnum = Enum.Parse<Skill>(skill);
                request.Skill = skillEnum;
                var typeName = typeParts.Length > 1 ? typeParts[1] : "";
                var questionType = await _unitOfWork.QuestionTypes.GetAsync(x => x.Skill == skillEnum && x.TypeName == typeName);
                if (questionType != null)
                    request.QuestionTypeId = questionType.Id;
                else
                    return null;
            }

            // Parse Độ khó
            var difficultyText = sheet.Cells[row, 4].Text;
            if (!Enum.TryParse<Difficulty>(difficultyText, out var difficulty))
                return null;
            request.Difficulty = difficulty;

            // Parse Nội dung câu hỏi
            request.QuestionContent = sheet.Cells[row, 5].Text;
            if (string.IsNullOrEmpty(request.QuestionContent))
                return null;

            // Parse Đáp án
            var correctAnswers = sheet.Cells[row, 6].Text.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            for (int i = 7; i <= 10; i++)
            {
                var answerText = sheet.Cells[row, i].Text;
                if (!string.IsNullOrEmpty(answerText))
                {
                    request.Answers.Add(new AddAnswerDTO
                    {
                        Content = answerText,
                        IsCorrect = correctAnswers.Contains($"{(char)(64 + (i - 6))}")
                    });
                }
            }
            if (request.Answers.Count < 2 || !request.Answers.Any(a => a.IsCorrect))
                return null;

            // Parse và so sánh file đính kèm
            var audioFileName = sheet.Cells[row, 11].Text;
            var imageFileName = sheet.Cells[row, 12].Text;
            if (!string.IsNullOrEmpty(audioFileName))
            {
                if (!attachments.ContainsKey(audioFileName))
                {
                    return null; // Tệp âm thanh không khớp
                }
                var audioFile = attachments[audioFileName];
                var audioExtension = Path.GetExtension(audioFile.FileName).ToLower();
                if (!new[] { ".mp3", ".wav" }.Contains(audioExtension))
                    return null;
                if (audioFile.Length > 10 * 1024 * 1024) // 10MB
                    return null;
                request.AttachmentFileAudio = audioFile;
            }
            if (!string.IsNullOrEmpty(imageFileName))
            {
                if (!attachments.ContainsKey(imageFileName))
                {
                    return null; // Tệp ảnh không khớp
                }
                var imageFile = attachments[imageFileName];
                var imageExtension = Path.GetExtension(imageFile.FileName).ToLower();
                if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(imageExtension))
                    return null;
                if (imageFile.Length > 5 * 1024 * 1024) // 5MB
                    return null;
                request.AttachmentFileImage = imageFile;
            }

            return request;
        }
        private async Task<AddSingleQuestionRequest> ParseEssayRow(ExcelWorksheet sheet, int row, Guid requestedUserId, Dictionary<string, IFormFile> attachments)
        {
            var request = new AddSingleQuestionRequest
            {
                RequestedUserId = requestedUserId,
                IsMultipleChoice = false,
                Answers = new List<AddAnswerDTO>()
            };

            // Parse Nguồn
            var refText = sheet.Cells[row, 1].Text;
            if (!string.IsNullOrEmpty(refText))
            {
                var refParts = refText.Split('-');
                var refName = refParts[0];
                var refs = await _unitOfWork.References.GetAsync(x => x.ReferenceName == refName);
                if (refs != null)
                    request.ReferenceId = refs.Id;
                else
                    return null;
            }

            // Parse Chủ đề
            var levelText = sheet.Cells[row, 2].Text;
            if (!string.IsNullOrEmpty(levelText))
            {
                var levelParts = levelText.Split('-');
                var levelName = levelParts[0];
                var topicName = levelParts.Length > 1 ? levelParts[1] : "";
                var levelDetail = await _unitOfWork.LevelDetails.GetAsync(x => x.Level.LevelName == levelName && x.Topic.TopicName == topicName);
                if (levelDetail != null)
                    request.LevelDetailId = levelDetail.Id;
                else
                    return null;
            }

            // Parse Loại câu hỏi
            var typeText = sheet.Cells[row, 3].Text;
            if (!string.IsNullOrEmpty(typeText))
            {
                //var typeParts = typeText.Split('-');
                var typeParts = typeText.Split('-');
                var skill = typeParts[0];
                var typeName = typeParts.Length > 1 ? typeParts[1] : "";
               // var questionType = await _unitOfWork.QuestionTypes.GetAsync(x => x.Skill.ToString() == skill && x.TypeName == typeName);
                var questionType = await _unitOfWork.QuestionTypes.GetAsync(x => x.TypeName == typeName);
                if (questionType != null)
                    request.QuestionTypeId = questionType.Id;
                else
                    return null;
            }

            // Parse Độ khó
            var difficultyText = sheet.Cells[row, 4].Text;
            if (!Enum.TryParse<Difficulty>(difficultyText, out var difficulty))
                return null;
            request.Difficulty = difficulty;

            // Parse Nội dung câu hỏi
            request.QuestionContent = sheet.Cells[row, 5].Text;
            if (string.IsNullOrEmpty(request.QuestionContent))
                return null;

            // Parse Đáp án
            var answerText = sheet.Cells[row, 6].Text;
            if (!string.IsNullOrEmpty(answerText))
            {
                request.Answers.Add(new AddAnswerDTO
                {
                    Content = answerText,
                    IsCorrect = true
                });
            }
            else
                return null;

            // Parse và so sánh file đính kèm
            var audioFileName = sheet.Cells[row, 7].Text;
            var imageFileName = sheet.Cells[row, 8].Text;
            if (!string.IsNullOrEmpty(audioFileName))
            {
                if (!attachments.ContainsKey(audioFileName))
                {
                    return null; // Tệp âm thanh không khớp
                }
                var audioFile = attachments[audioFileName];
                var audioExtension = Path.GetExtension(audioFile.FileName).ToLower();
                if (!new[] { ".mp3", ".wav" }.Contains(audioExtension))
                    return null;
                if (audioFile.Length > 10 * 1024 * 1024) // 10MB
                    return null;
                request.AttachmentFileAudio = audioFile;
            }
            if (!string.IsNullOrEmpty(imageFileName))
            {
                if (!attachments.ContainsKey(imageFileName))
                {
                    return null; // Tệp ảnh không khớp
                }
                var imageFile = attachments[imageFileName];
                var imageExtension = Path.GetExtension(imageFile.FileName).ToLower();
                if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(imageExtension))
                    return null;
                if (imageFile.Length > 5 * 1024 * 1024) // 5MB
                    return null;
                request.AttachmentFileImage = imageFile;
            }

            return request;
        }

        private async Task<APIResponse<ImportQuestionResultDTO>> AddQuestionAsync(AddQuestionRequest request, string ipAddress)
        {
            APIResponse<ImportQuestionResultDTO> response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            var requestedUser = request.RequestedUser;
            AddQuestionTask? targetTask = null;
            // Kiểm tra xem task có tồn tại không
            if (request.TaskId != null)
            {
                targetTask = await _unitOfWork.AddQuestionTasks.GetAsync(x => x.Id == request.TaskId);
                if (targetTask == null)
                {
                    response.Message = AppMessages.TASK_NOTFOUND;
                    return response;
                }
                if (targetTask.AssignId != requestedUser.Id)
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.Message = AppMessages.CANNOT_ACCESS_TASK_OF_OTHER_USER;
                    return response;
                }
            }
            // Khởi tạo log hành động
            SystemAccessLog questionLog = new()
            {
                UserId = requestedUser.Id,
                TargetObject = nameof(Question),
                ActionName = string.Format(AccessLogConstant.CREATE_ACTION, "câu hỏi bằng hình thức: ") + request.AddMethod,
                IsSuccess = false,
                AccessTime = DateTime.Now,
                IpAddress = request.IpAddress ?? "",
            };
            try
            {
                // Bắt đầu transaction
                await _unitOfWork.BeginTransactionAsync();

                // Ghi log hành động vào database
                await _unitOfWork.AccessLogs.AddAsync(questionLog);

                int count = 0;
                List<Guid> addedIds = new List<Guid>();

                // Thêm câu hỏi
                foreach (var item in request.Requests)
                {
                    //item.Task = targetTask;
                    //item.AccessLog = questionLog;
                    //item.RequestedUser = requestedUser;

                    // Thêm câu hỏi đơn lẻ
                    APIResponse<QuestionDetailDto> singleResponse;
                    //singleResponse = await AddSingleQuestionWithoutValid(item);

                    //if (!singleResponse.IsSuccess)
                    //{
                    //    response.Result = new List<ImportQuestionResultDTO>();
                    //    response.StatusCode = singleResponse.StatusCode;
                    //    throw new Exception($"Thêm câu hỏi lỗi tại vị trí {count + 1}: " + singleResponse.Message);
                    //}
                    //else
                    //{
                    //    response.Result.Add(new ImportQuestionResultDTO { Question = _mapper.Map<QuestionDetailDto>(singleResponse.Result.First()), Messages = new List<string> { singleResponse.Message } });
                    //    count++;
                    //}
                }
                // Gửi email thông báo sau khi hoàn thành thêm câu hỏi
                //await SendEmailToLeadAfterImportingQuestion(requestedUser, count);

                // Cập nhật log khi thành công
                questionLog.IsSuccess = true;
                questionLog.Details = $"{requestedUser.UserName} đã thêm {count} câu hỏi mới!";

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.SaveChangesAsync();

                // Commit transaction
                await _unitOfWork.CommitAsync();
                // Gửi email sau khi hoàn thành
                await Task.Run(() =>
                {
                    //_unitOfWork.EmailService.SendEmail(requestedUser.Email,
                    //    "YOU'VE JUST UPLOAD QUESTIONS TO THE FSAKEB SYSTEM",
                    //    $"You have just completed importing {request.Requests.Count} questions to the question bank. Method: {request.AddMethod}",
                    //    requestedUser.UserName);

                    //// Lên lịch xóa câu hỏi sau một thời gian
                    //_unitOfWork.Schedule<QuestionSimpleService>(
                    //    (x) => x.DeleteQuestionsOfLog(questionLog.Id),
                    //    DateTime.Now.AddDays(SystemDataFormat.QUESTION_STORING_DURATION_AFTER_IMPORTING));
                });

                response.StatusCode = System.Net.HttpStatusCode.Created;
                response.Message = AppMessages.QUESTIONS_ARE_PROCESSING;
                response.IsSuccess = true;
            }
            catch (Exception e)
            {
                // Nếu có lỗi trong quá trình thêm câu hỏi, rollback transaction
                Console.WriteLine(e.Message);
                await _unitOfWork.RollbackAsync();
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Message = e.Message;
                response.Result = new List<ImportQuestionResultDTO>();
            }
            return response;

        }

        public async Task<APIResponse<GetDuplicateQuestionResultDTO>> FindDuplicateQuestions(Guid questionId)
        {
            APIResponse<GetDuplicateQuestionResultDTO> response = new();
            try
            {
                var question = await _unitOfWork.Questions.GetAsync(x => x.Id == questionId,
                                                                            includeProperties: "QuestionType,Answers,References," +
                                                                                    "LevelDetail,LevelDetail.Level,LevelDetail.Topic");
                var existingQuestions = await _unitOfWork.Questions.GetAllAsync(
                filter: x => x.Id != questionId, // Exclude the current question
                includeProperties: "QuestionType,Answers,References,LevelDetail,LevelDetail.Level,LevelDetail.Topic",
                asTracking: false
            );
                // var duplicateQuestion = existingQuestions
                //.FirstOrDefault(q => q.QuestionContent.Trim().ToLower() == question.QuestionContent.Trim().ToLower()
                //);
                var duplicateQuestion = existingQuestions.FirstOrDefault(q =>
                {
                    // Compare question content
                    bool contentMatches = q.QuestionContent.Trim().ToLower() == question.QuestionContent.Trim().ToLower();

                    // Compare answers (assuming order doesn't matter)
                    bool answersMatch = q.Answers != null && question.Answers != null &&
                        q.Answers.Count == question.Answers.Count &&
                        q.Answers.Select(a => a.AnswerContent.Trim().ToLower())
                            .OrderBy(a => a)
                            .SequenceEqual(question.Answers.Select(a => a.AnswerContent.Trim().ToLower()).OrderBy(a => a));

                    return contentMatches && answersMatch;
                });
                var result = _mapper.Map<GetDuplicateQuestionResultDTO>(question);
                await _unitOfWork.BeginTransactionAsync();
                if (duplicateQuestion != null)
                {
                    // Update status to duplicated
                    //question.Status = QuestionStatus.Duplicated;
                    await _unitOfWork.Questions.UpdateWithNoCommitAsync(question);
                    await _unitOfWork.CommitAsync();

                    result.Message = $"Question is duplicated with question ID: {duplicateQuestion.Id}";
                    result.Question.Status = "duplicated";
                }
                else
                {
                    result.Message = "No duplicate question found";
                }
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result[0] = result;
                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
        }

        public async Task<APIResponse<QuestionDetailDto>> AddSingleQuestionAsync(AddSingleQuestionRequest request)
        {
            var response = new APIResponse<QuestionDetailDto>();
            try
            {
                // Validate request
                if (string.IsNullOrEmpty(request.QuestionContent))
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Nội dung câu hỏi không được để trống";
                    return response;
                }
                
                var attach = new ImageFile();
                if (request.AttachmentFileImage != null)
                {
                    attach = await GetAttachFile.GetImageFile(request.AttachmentFileImage);
                    await _unitOfWork.ImageFiles.AddAsync(attach);
                }
                var attachment = new ImageFile();
                if (request.AttachmentFileAudio != null)
                {
                    attachment = await GetAttachFile.GetImageFile(request.AttachmentFileAudio);
                    await _unitOfWork.ImageFiles.AddAsync(attachment);
                }
                // Create question entity from DTO

                var question = _mapper.Map<Question>(request);
                question.Id = Guid.NewGuid();
                question.CreatedBy = request.RequestedUserId;
                question.UpdatedBy = request.RequestedUserId;
                question.CreatedDate = DateTime.Now;
                question.UpdatedDate = DateTime.Now;
                question.LevelDetailId = request.LevelDetailId;
                question.QuestionTypeId = request.QuestionTypeId;
                question.ReferenceId = request.ReferenceId;
                question.Status = QuestionStatus.Pending;
                if (request.AttachmentFileImage != null)
                {
                    question.AttachFileImageId = attach.Id;
                }
                if (request.AttachmentFileAudio != null)
                {
                    question.AttachFileAudioId = attachment.Id;
                }
                // Handle log creation
                var systemAccessLog = new SystemAccessLog
                {
                    Id = Guid.NewGuid(),
                    IpAddress = "",
                    AccessTime = DateTime.UtcNow,
                    TargetObject = "Question",
                    ActionName = "Add",
                    Details = $"Thêm câu hỏi: {request.QuestionContent}",
                    IsSuccess = true, // Gán false nếu gặp lỗi
                    UserId = request.RequestedUserId
                };
                await _unitOfWork.AccessLogs.AddAsync(systemAccessLog);
                question.LogId = systemAccessLog.Id;
                
                // Handle task creation (if needed)
                if (request.TaskId.HasValue)
                {
                    var task = await _unitOfWork.AddQuestionTasks.GetByIdAsync(request.TaskId.Value);
                    if (task != null)
                    {
                        question.TaskId = task.Id;
                    }
                    else
                    {
                        // If task doesn't exist, handle it (create new task or return an error)
                        response.IsSuccess = false;
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = "Tác vụ không hợp lệ";
                        return response;
                    }
                }

                question.Description = "";
                // Lưu vào DB
                await _unitOfWork.Questions.AddAsync(question);

                // Validate theo loại câu hỏi
                if (request.IsMultipleChoice)
                {
                    if (request.Answers.Count < 2)
                    {
                        response.IsSuccess = false;
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = "Câu hỏi trắc nghiệm phải có ít nhất 2 đáp án";
                        return response;
                    }

                    if (!request.Answers.Any(a => a.IsCorrect))
                    {
                        response.IsSuccess = false;
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = "Phải có ít nhất một đáp án đúng";
                        return response;
                    }

                    // Tạo danh sách answers mới
                    foreach (var x in request.Answers)
                    {
                        var answer = new Answer
                        {
                            Id = Guid.NewGuid(),
                            AnswerContent = x.Content,
                            IsTrue = x.IsCorrect,
                            QuestionId = question.Id,
                        };
                        await _unitOfWork.Answers.AddAsync(answer);
                    }
                }
                else // tự luận
                {
                    if (request.Answers.Count != 1)
                    {
                        response.IsSuccess = false;
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = "Câu hỏi tự luận phải có đúng một đáp án";
                        return response;
                    }

                    // Dùng foreach và await từng tác vụ
                    foreach (var x in request.Answers)
                    {
                        var answer = new Answer
                        {
                            Id = Guid.NewGuid(),
                            AnswerContent = x.Content,
                            IsTrue = x.IsCorrect,
                            QuestionId = question.Id,
                        };
                        await _unitOfWork.Answers.AddAsync(answer);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
                    //gửi mail thông báo
                    var actor = await _unitOfWork.Users.GetByIdAsync(request.RequestedUserId);
                if (request.TaskId.HasValue)
                {
                    var task = await _unitOfWork.AddQuestionTasks.GetByIdAsync(request.TaskId.Value);
                    if (task != null)
                    {
                        question.TaskId = task.Id;

                        // Lấy thông tin người tạo task
                        var taskCreator = await _unitOfWork.Users.GetByIdAsync(task.CreatedBy.Value);
                        if (taskCreator != null)
                        {
                            _unitOfWork.EmailService.SendEmail(
                                taskCreator.Email,
                                "HỆ THỐNG CÓ CÂU HỎI MỚI ĐƯỢC THÊM VÀO TASK CỦA BẠN",
                                $"{actor?.UserName} vừa thêm một câu hỏi mới vào tác vụ bạn đã tạo lúc {DateTime.Now:dd/MM/yyyy HH:mm}. " +
                                "Vui lòng truy cập hệ thống để kiểm tra và duyệt câu hỏi.",
                                taskCreator.FullName
                            );
                            //noti
                            await _unitOfWork.Notifications.AddAsync(new Notification
                            {
                                UserId = taskCreator.Id,
                                CreatedDate = DateTime.Now,
                                Description = $"Quản lý {actor.UserName} " +
                                             $"đã thêm câu hỏi vào tác vụ bạn đã tạo lúc {DateTime.Now:dd/MM/yyyy HH:mm}"
                            });
                        }
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = "Tác vụ không hợp lệ";
                        return response;
                    }
                }
                // Load thêm thông tin để trả về DTO đầy đủ
                var addedQuestion = await _unitOfWork.Questions.GetQuestionDetailByIdAsync(question.Id);
                if (addedQuestion == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "Không tìm thấy câu hỏi sau khi thêm";
                    return response;
                }

                var result = _mapper.Map<QuestionDetailDto>(addedQuestion);
                response.Result = new List<QuestionDetailDto> { result };
                response.IsSuccess = true;
                response.Message = "Thêm câu hỏi thành công";
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = $"Lỗi hệ thống: {ex.Message}";
                Console.WriteLine($"Error in AddSingleQuestionAsync: {ex}");
            }

            return response;
        }


    }
}