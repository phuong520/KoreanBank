using AutoMapper;
using KEB.Application.DTOs.AnswerDTO;
using KEB.Application.DTOs.Common;
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
using Newtonsoft.Json;
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
    public class QuestionService : IQuestionService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public QuestionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponse<QuestionDetailDto>> GetQuestionDetailAsync(Guid questionId)
        {
            APIResponse<QuestionDetailDto> response = new();
            try
            {
                var queriedQuestion = await _unitOfWork.Questions
                    .GetAsync(x => x.Id == questionId, "QuestionType,Answers,CreatedBy,UpdatedBy," +
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

            var queryResult = await _unitOfWork.Questions.GetAllAsync(
                    filter: filter,
                    includeProperties: "QuestionType,Answers,LevelDetail,References,LevelDetail.Level,LevelDetail.Topic",
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
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            try
            {
                var requestedUser = await _unitOfWork.Users.GetUserById(request.RequestedUserId);
                if (requestedUser == null || requestedUser.RoleId.ToString() != LogicString.Role.TeamLeadRoleId)
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.Message = AppMessages.NO_PERMISSION;
                    return response;
                }
                List<string> validError = [];
                var importHistory = await _unitOfWork.AccessLogs.GetAsync(x => x.Id == request.LogId && x.TargetObject == nameof(Question),
                                                                includeProperties: "User");
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
                                                orderBy: x => x.OrderBy(x => x.Id),
                                                includeProperties: "Answers")).ToList();
                // All questions to be processed must exist in the system
                if (targetQuestions.Count < idList.Count())
                {
                    validError.Add("Some questions can not be found to be processed");
                }
                if (targetQuestions.Any(x => x.LogId != request.LogId))
                {
                    validError.Add("Can only approve/deny questions in the same import history");
                }
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
                    Description = $"Team lead {requestedUser.UserName} " +
                                  $"has approved {approved} questions and denied {denied} question of the import history on " +
                                  $"{importHistory.ActionName:dd MMM yyyy}",
                });

                // Send email to the created user
                await Task.Run(() =>
                {
                    _unitOfWork.EmailService.SendEmail(importHistory.User.Email,
                                                "TEAM LEAD HAS MADE SOME UPDATE ON YOUR QUESTIONS",
                                                $"Team lead {requestedUser.UserName} " +
                                                        $"has approved {approved} questions and denied {denied} question of the import history on {importHistory.ActionName:dd MMM yyyy}",
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

        public Task<APIResponse<GetDuplicateQuestionResultDTO>> FindDuplicateQuestions(Guid questionId)
        {
            throw new NotImplementedException();
        }

        public async Task<APIResponse<object>> UpdateQuestion(UpdateQuestionRequest request)
        {
            var response = new APIResponse<object> { IsSuccess = false };
            try
            {
                var question = await _unitOfWork.Questions.GetAsync(
                    x => x.Id == request.TargetObjectId,
                    includeProperties: "QuestionType,Answers,LevelDetail,Reference,LevelDetail.Level,LevelDetail.Topic");

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
                response.Result.Add(result);
                response.IsSuccess = true;
                response.Message = "Cập nhật thành công";
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (Exception)
            {
                response.Message = "Có lỗi xảy ra khi cập nhật câu hỏi";
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }

        public Task<APIResponse<ImportQuestionResultDTO>> UploadQuestionFromExcel(ImportQuestionFromExcelRequest request, string ipAddress)
        {
            throw new NotImplementedException();
        }


        private async Task<APIResponse<ImportQuestionResultDTO>> AddQuestionAsync(AddQuestionRequest request, string ipAddress)
        {
            APIResponse<ImportQuestionResultDTO> response = new() { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };
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
                // ActionName = string.Format(AccessLogConstant.CREATE_ACTION, "câu hỏi bằng hình thức: ") + request.AddMethod,
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
                    item.Task = targetTask;
                    item.AccessLog = questionLog;
                    item.RequestedUser = requestedUser;

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
                await SendEmailToLeadAfterImportingQuestion(requestedUser, count);

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
        private async Task SendEmailToLeadAfterImportingQuestion(User actor, int numOfQues)
        {
            var leads = await _unitOfWork.Users.GetAllAsync(x => x.RoleId.ToString() == LogicString.Role.TeamLeadRoleId);
            foreach (var lead in leads)
            {
                await Task.Run(() =>
                {
                    _unitOfWork.EmailService.SendEmail(
                        toEmail: lead.Email,
                        subject: SystemDataFormat.INFORM_NEWQUESTION_EMAIL_SUBJECT,
                        body: string.Format(SystemDataFormat.INFORM_NEWQUESTION_EMAIL_BODY, actor.UserName, numOfQues),
                        receiverName: lead.UserName);
                });
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
                if (request.AttachmentFile != null)
                {
                    attach = await GetAttachFile.GetImageFile(request.AttachmentFile);
                    await _unitOfWork.ImageFiles.AddAsync(attach);
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
                if (request.AttachmentFile != null)
                {
                    question.AttachFileId = attach.Id;
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