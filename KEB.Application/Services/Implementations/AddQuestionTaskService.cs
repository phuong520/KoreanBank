using AutoMapper;
using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ImportQuestionTaskDTO;
using KEB.Application.DTOs.SystemAccessLogDTO;
using KEB.Application.Services.Interfaces;
using KEB.Application.Utils;
using KEB.Domain.Entities;
using KEB.Domain.Enums;
using KEB.Domain.ValueObject;
using KEB.Infrastructure.ExternalService.IExternalImplementation;
using KEB.Infrastructure.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Implementations
{
    public class AddQuestionTaskService : IAddQuestionTaskService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public AddQuestionTaskService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponse<TaskGeneralDisplayDTO>> AssignImportQuestionTask(AssignTaskRequest request)
        {
            APIResponse<TaskGeneralDisplayDTO> response = new()
            {
                IsSuccess = false,
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Result = new()
            };
            List<string> scheduledBackgroundJobIdsList = [];
            try
            {
                var requestedUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.RequestedUserId);
                if (requestedUser == null || requestedUser.RoleId.ToString() != LogicString.Role.TeamLeadRoleId)
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.Message = AppMessages.NO_PERMISSION;
                    return response;
                }

                if (request.TasksList.Count == 0)
                {
                    response.Message = AppMessages.EMPTY_TASK_LIST;
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    return response;
                }

                var assignee = await _unitOfWork.Users.GetAsync(x => x.Id == request.AssigneeId);
                if (assignee == null)
                {
                    response.Message = AppMessages.ASSIGNEE_NOT_EXISTED;
                    return response;
                }
                if (assignee.RoleId.ToString() != LogicString.Role.LecturerRoleId && assignee.Id != requestedUser.Id)
                {
                    response.IsSuccess = false;
                    response.Message = "Chỉ được giao nhiệm vụ cho giảng viên!";
                    return response;
                }
                if (!assignee.IsActive)
                {
                    response.IsSuccess = false;
                    response.Message = AppMessages.ASSIGNEE_LOCKED;
                    return response;
                }

                int currentOrderOfTask = _unitOfWork.AddQuestionTasks.FinalTaskIndex + 1;
                DateTime currentTime = DateTime.Now;
                string actionDetails = $"{requestedUser.UserName} đã giao {request.TasksList.Count} nhiệm vụ cho {assignee.UserName} ";
                string emailSubject = SystemDataFormat.INFORM_NEWTASK_EMAIL_SUBJECT;
                string tasksDetailEmail = "";

                var addedTasksList = new List<AddQuestionTask>();
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    foreach (var task in request.TasksList)
                    {
                        List<string> validTaskResult = [];
                        var levelDetail = await _unitOfWork.LevelDetails.GetAsync(x => x.Id == task.LevelDetailId, includeProperties: "Topic,Level");
                        if (levelDetail == null) validTaskResult.Add("Không tìm thấy trình độ");
                        var questionType = await _unitOfWork.QuestionTypes.GetAsync(x => x.Id == task.QuestionTypeId);
                        if (questionType == null) validTaskResult.Add("Không tìm thấy loại câu hỏi");
                        if (task.NumberOfQuestions < 1) validTaskResult.Add("Số lượng câu hỏi không hợp lệ cho nhiệm vụ");
                        if (task.Deadline < currentTime.AddDays(3)) validTaskResult.Add("Deadline của nhiệm vụ phải sau ít nhất 3 ngày");
                        if (validTaskResult.Count > 0)
                        {
                            throw new VersionNotFoundException($"~ Nhiệm vụ thứ {currentOrderOfTask} có vấn đề: " + string.Join(" ~ ", validTaskResult));
                        }

                        string taskName = string.Format("TASK_" + currentOrderOfTask);
                        AddQuestionTask newTask = new()
                        {
                            Id = Guid.NewGuid(),
                            AssignId = request.AssigneeId,
                            //User = assignee,
                            TaskName = taskName,
                            DeadLine = task.Deadline,
                            Difficult = task.Difficulty,
                            LevelDetailId = task.LevelDetailId,
                            QuestionTypeId = task.QuestionTypeId,
                            ForMultiChoice = task.ForMultipleChoice,
                            NumberOfQuestion = task.NumberOfQuestions,
                            Status = AddQuestionStatus.InProgress,
                            CreatedDate = currentTime,
                            UpdatedDate = currentTime,
                            CreatedBy = request.RequestedUserId,
                            UpdatedBy = request.RequestedUserId,
                        };

                        await _unitOfWork.AddQuestionTasks.AddAsync(newTask);
                        await _unitOfWork.SaveChangesAsync(); // Save after each task to ensure proper ID generation
                        addedTasksList.Add(newTask);
                        response.Result.Add(_mapper.Map<TaskGeneralDisplayDTO>(newTask));

                        tasksDetailEmail += string.Format(
                            newTask.ForMultiChoice ? SystemDataFormat.TASK_MULTIPLECHOCIE_INFO_FORMAT : SystemDataFormat.TASK_ESSAY_INFO_FORMAT,
                            taskName,
                            task.Deadline.ToString("yyyy-MM-dd"),
                            levelDetail.Topic.TopicName,
                            task.Difficulty.ToString(),
                            task.NumberOfQuestions,
                            questionType.Skill.ToString(),
                            questionType.TypeName
                        );
                        currentOrderOfTask++;
                    }

                    //// Schedule reminder emails
                    //for (int i = 0; i < addedTasksList.Count; i++)
                    //{
                    //    var task = addedTasksList[i];
                    //    string? jobId = await AutoSendReminderEmailBeforeDeadLine(new RemindTaskDeadlineRequest
                    //    {
                    //        AssigneeEmail = assignee.Email,
                    //        AssigneeName = assignee.UserName,
                    //        Deadline = task.DeadLine,
                    //        TaskDetails = tasksDetailEmail,
                    //        TaskName = task.TaskName,
                    //        TaskId = task.Id,
                    //    });
                    //    task.ScheduleTaskId = jobId;
                    //    scheduledBackgroundJobIdsList.Add(jobId ?? "");
                    //    await _unitOfWork.SaveChangesAsync(); // Save after updating schedule task ID
                    //}

                    // Add notification
                    var notification = new Notification
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        Description = $"{requestedUser.UserName} đã giao cho bạn {addedTasksList.Count} nhiệm vụ mới",
                        UserId = request.AssigneeId,
                        CreatedBy = request.RequestedUserId,
                        UpdatedBy = request.RequestedUserId,
                        UpdatedDate = DateTime.Now
                    };
                    await _unitOfWork.Notifications.AddAsync(notification);

                    // Add access log
                    var accessLog = new SystemAccessLog
                    {
                        Id = Guid.NewGuid(),
                        IsSuccess = true,
                        AccessTime = currentTime,
                        ActionName = $"Giao nhiệm vụ cho {assignee.UserName}",
                        IpAddress = request.IpAddress ?? "::1",
                        UserId = request.RequestedUserId,
                        TargetObject = nameof(AddQuestionTask),
                        Details = actionDetails,
                        CreatedBy = request.RequestedUserId,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        UpdatedBy = request.RequestedUserId
                    };
                    await _unitOfWork.AccessLogs.AddAsync(accessLog);

                    string emailBody = string.Format(SystemDataFormat.INFORM_NEWTASK_EMAIL_BODY,
                                     assignee.UserName,
                                     requestedUser.UserName,
                                     request.TasksList.Count,
                                     tasksDetailEmail);

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();

                    // Send notification to assignee by email
                    _unitOfWork.EmailService.SendEmail(assignee.Email, emailSubject, emailBody, assignee.UserName);

                    response.Message = AppMessages.TASK_ASSIGNED_SUCCESS;
                    response.IsSuccess = true;
                    response.StatusCode = System.Net.HttpStatusCode.Created;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                    throw; // Re-throw to be caught by outer catch block
                }
            }
            catch (Exception ex)
            {
                // Roll back thì cần xóa những task đã được schedule trước đó đi
                //foreach (var jobId in scheduledBackgroundJobIdsList)
                //{
                //    if (!string.IsNullOrEmpty(jobId))
                //    {
                //        await Task.Run(() => _unitOfWork.DeleteScheduledJob(jobId));
                //    }
                //}
                response.Message = ex.Message;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return response;
        }
        private async Task<string?> AutoSendReminderEmailBeforeDeadLine(RemindTaskDeadlineRequest request)
        {
            string emailSubject = SystemDataFormat.INFORM_DEADLINE_APPROACHING_EMAIL_SUBJECT;
            string emailBody = string.Format(SystemDataFormat.INFORM_DEADLINE_APPROACHING_EMAIL_BODY,
                                request.AssigneeName,
                                request.TaskName,
                                request.Deadline.ToString(),
                                request.TaskDetails);

            DateTimeOffset enqueueTime = new(request.Deadline.AddHours(SystemDataFormat.NUMOFHOURS_BEFORE_DEADLINE_TO_ALERT), TimeSpan.Zero);

            string jobid = await Task.Run(() => _unitOfWork.Schedule<EmailService>(x => x.SendEmail(request.AssigneeEmail,
                                                                                emailSubject,
                                                                                emailBody,
                                                                                request.AssigneeName),
                                                                         enqueueTime));
            await Console.Out.WriteLineAsync("AutoSendReminderEmailBeforeDeadLine has been called ~");
            return jobid;

        }
        public async Task<APIResponse<TaskGeneralDisplayDTO>> ChangeTaskDeadline(ChangeTaskDeadlineRequest request)

        {
            APIResponse<TaskGeneralDisplayDTO> response = new() { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };
            // Validate data before editing start
            try
            {
                var requestedUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.RequestedUserId);
                if (requestedUser == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.Message = AppMessages.NO_PERMISSION;
                    return response;
                }
                var targetTask = await _unitOfWork.AddQuestionTasks.GetAsync(x => x.Id == request.TargetTaskId,
                                                    includeProperties: "QuestionType,Assignee,LevelDetail,LevelDetail.Topic");
                if (targetTask == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                    return response;
                }

                var assignee = targetTask.User;
                DateTime currentTime = DateTime.Now;
                DateTime currentDeadLine = targetTask.DeadLine;
                DateTime minTime = currentTime.AddDays(SystemDataFormat.MIN_TIME_TO_ASSIGN_TASK_IN_DAYS).AddMinutes(1);
                if (request.NewDeadLine < minTime)
                {
                    response.Message = AppMessages.DEADLINE_TOO_HURRY;
                }
                else
                {
                    if (targetTask.ScheduleTaskId != null)
                        await Task.Run(() => _unitOfWork.DeleteScheduledJob(targetTask.ScheduleTaskId));

                    targetTask.DeadLine = request.NewDeadLine;
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                    {
                        AccessTime = currentTime,
                        TargetObject = nameof(AddQuestionTask),
                        ActionName = "Thay đổi deadline",
                        IpAddress = request.IpAddress ?? "",
                        IsSuccess = true,
                        UserId = request.RequestedUserId,
                        Details = $"{requestedUser.UserName} đã thay đổi deadline của nhiệm vụ {targetTask.TaskName} " +
                                  $"từ {currentDeadLine} sang {request.NewDeadLine}"
                    });

                    //await Task.Run(() => _unitOfWork.EmailService.SendEmail(assignee.Email,
                    //                                                        "THỜI HẠN CHO NHIỆM VỤ CỦA BẠN ĐÃ THAY ĐỔI",
                    //                                                        $"Deadline cho nhiệm vụ {targetTask.TaskName} đã được thay đổi",
                    //                                                        assignee.UserName));

                    string newTaskDetails = string.Format(
                        targetTask.ForMultiChoice ? SystemDataFormat.TASK_MULTIPLECHOCIE_INFO_FORMAT : SystemDataFormat.TASK_ESSAY_INFO_FORMAT,
                        targetTask.TaskName,
                        targetTask.DeadLine.ToString("yyyy-MM-dd"),
                        targetTask.LevelDetail.Topic.TopicName,
                        targetTask.Difficult.ToString(),
                        targetTask.NumberOfQuestion,
                        targetTask.QuestionType.Skill.ToString(),
                        targetTask.QuestionType.TypeName
                    );

                    targetTask.ScheduleTaskId = await AutoSendReminderEmailBeforeDeadLine(new RemindTaskDeadlineRequest
                    {
                        AssigneeEmail = assignee.Email,
                        AssigneeName = assignee.UserName,
                        Deadline = targetTask.DeadLine,
                        TaskDetails = newTaskDetails,
                        TaskName = targetTask.TaskName,
                        TaskId = targetTask.Id,
                    });
                    response.IsSuccess = true;
                    response.Message = AppMessages.TASK_UPDATE_SUCCESS;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;
        }

        public async Task<APIResponse<TaskGeneralDisplayDTO>> DeleteImportQuestionTask(Delete request)
        {
            APIResponse<TaskGeneralDisplayDTO> response = new() { IsSuccess = false };
            // Validate data before editing start
            try
            {
                var requestedUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.RequestedUserId);
                if (requestedUser == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.Message = AppMessages.NO_PERMISSION;
                    return response;
                }

                var (DeletedTask, RelatedQuestions) = await _unitOfWork.AddQuestionTasks.DeleteTaskAsync(request.TargetObjectId);

                if (DeletedTask == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                    return response;
                }
                else
                {
                    response.Result.Add(_mapper.Map<TaskGeneralDisplayDTO>(DeletedTask));
                    if (RelatedQuestions > 0)
                    {
                        //response.Message = $"Không được xóa nhiệm vụ này " +
                        //        $"vì người dùng {DeletedTask.Assignee.UserName} " +
                        //        $"đã thêm {RelatedQuestions} câu hỏi vào rồi ~";
                        response.Message = AppMessages.TASK_IS_IN_PROCESS;
                        response.StatusCode = System.Net.HttpStatusCode.Conflict;
                    }
                    else
                    {
                        if (DeletedTask.ScheduleTaskId != null)
                            await Task.Run(() => _unitOfWork.DeleteScheduledJob(DeletedTask.ScheduleTaskId));
                        //response.Message = $"Đã xóa nhiệm vụ. ";
                        response.Message = AppMessages.TASK_DELETE_SUCCESS;
                        response.IsSuccess = true;
                    }
                }
            }
            catch (Exception)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;
        }

        public async Task<APIResponse<TaskGeneralDisplayDTO>> EditImportQuestionTask(EditTaskRequest request)

        {
            APIResponse<TaskGeneralDisplayDTO> response = new() { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };
            try
            {
                // Validate data before editing start
                var requestedUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.RequestedUserId);
                if (requestedUser == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.Message = AppMessages.NO_PERMISSION;
                    return response;
                }
                // Nếu 0 tìm thấy target task, trả về not found
                var targetTask = await _unitOfWork.AddQuestionTasks.GetAsync(x => x.Id == request.TaskId,
                                                    includeProperties: "QuestionType,Questions,Assignee,LevelDetail,LevelDetail.Topic");
                if (targetTask == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                    return response;
                }

                // Nếu đã có câu hỏi link với nhiệm vụ, hong cho sửa
                if (targetTask.Questions.Any())
                {
                    response.Message = AppMessages.TASK_IS_IN_PROCESS;
                    return response;
                }

                User oldAssignee = targetTask.User;
                var newAssignee = oldAssignee;
                if (targetTask.User.Id != request.RequestedUserId)
                {
                    newAssignee = await _unitOfWork.Users.GetAsync(x => x.Id == request.AssigneeId);
                    if (newAssignee == null)
                    {
                        response.Message = AppMessages.ASSIGNEE_NOT_EXISTED;
                        return response;
                    }
                    if (!newAssignee.IsActive)
                    {
                        response.Message = AppMessages.ASSIGNEE_LOCKED;
                        return response;
                    }
                }
                bool changed = false;
                bool deadlineChanged = false;
                DateTime currentTime = DateTime.Now;
                // Validate data before editing end

                // Declare variables start
                List<string> actionDetails = [$"{requestedUser.UserName} chỉnh sửa nhiệm vụ {targetTask.TaskName}"];
                string newTaskDetails = "";
                string oldTaskDetails = string.Format(targetTask.ForMultiChoice ? SystemDataFormat.TASK_MULTIPLECHOCIE_INFO_FORMAT : SystemDataFormat.TASK_ESSAY_INFO_FORMAT,
                                                    targetTask.TaskName,
                                                    targetTask.DeadLine.ToString("yyyy-MM-dd"),
                                                    targetTask.LevelDetail.Topic.TopicName,
                                                    targetTask.Difficult.ToString(),
                                                    targetTask.NumberOfQuestion,
                                                    targetTask.QuestionType.Skill.ToString(),
                                                    targetTask.QuestionType.TypeName); ;

                var levelDetail = await _unitOfWork.LevelDetails.GetAsync(x => x.Id == request.LevelDetailId,
                                                                          includeProperties: "Topic")
                                                    ?? throw new Exception("~ Không tìm thấy chủ đề cho nhiệm vụ này  ~");
                var questionType = await _unitOfWork.QuestionTypes.GetAsync(x => x.Id == request.QuestionTypeId)
                                                    ?? throw new Exception("~ Không tìm thấy loại câu hỏi cho nhiệm vụ này ~");
                // Declare variables end

                // Perform change data & write out change details
                {
                    if (request.AssigneeId != null && targetTask.AssignId != request.AssigneeId)
                    //if (targetTask.AssigneeId != request.AssigneeId)
                    {
                        actionDetails.Add($"Đổi người nhận nhiệm vụ từ {oldAssignee.UserName} sang cho {newAssignee.UserName}");
                        targetTask.AssignId = (Guid)request.AssigneeId;
                        targetTask.User = newAssignee;
                        changed = true;
                    }
                    if (request.LevelDetailId != null && targetTask.LevelDetailId != request.LevelDetailId)
                    {
                        actionDetails.Add($"Đổi chủ đề thành {levelDetail.Topic.TopicName}");
                        targetTask.LevelDetail = levelDetail;
                        targetTask.LevelDetailId = (Guid)request.LevelDetailId;
                        changed = true;
                    }
                    if (request.QuestionTypeId != null && targetTask.QuestionTypeId != request.QuestionTypeId)
                    {
                        actionDetails.Add($"Đổi loại câu hỏi thành {questionType.TypeName}");
                        targetTask.QuestionType = questionType;
                        targetTask.QuestionTypeId = (Guid)request.QuestionTypeId;
                        changed = true;
                    }
                    if (request.ForMultipleChoice != null && targetTask.ForMultiChoice != request.ForMultipleChoice)
                    {
                        actionDetails.Add("Đổi nhiệm vụ thành " + ((bool)request.ForMultipleChoice ? "trắc nghiệm." : "tự luận"));
                        targetTask.ForMultiChoice = (bool)request.ForMultipleChoice;
                        changed = true;
                    }
                    if (request.Difficulty != null && targetTask.Difficult != request.Difficulty)
                    {
                        actionDetails.Add($"Sửa độ khó từ {targetTask.Difficult} thành {request.Difficulty}");
                        targetTask.Difficult = (Difficulty)request.Difficulty;
                        changed = true;
                    }
                    if (request.NumberOfQuestions != null && targetTask.NumberOfQuestion != request.NumberOfQuestions)
                    {
                        actionDetails.Add($"Thay đổi số lượng câu hỏi: {targetTask.NumberOfQuestion} thành {request.NumberOfQuestions}");
                        targetTask.NumberOfQuestion = (int)request.NumberOfQuestions;
                        changed = true;
                    }
                    if (request.Deadline != null && targetTask.DeadLine != request.Deadline)
                    {
                        if (request.Deadline < currentTime.AddDays(1) && changed)
                        {
                            response.Message = $"{currentTime:yyyy-MM-dd} rồi, đòi nộp deadline vào " +
                                               $"{request.Deadline:yyyy-MM-dd} thì ai mà làm nổi!?!??" +
                                               $"Deadline sau khi sửa phải là ít nhất 1 ngày từ bây giờ ~";
                            return response;
                        }
                        if (request.Deadline < currentTime)
                        {
                            response.Message = $"{currentTime:yyyy-MM-dd} rồi, cho deadline vào " +
                                               $"{request.Deadline:yyyy-MM-dd} thì đi về quá khứ mà làm hả!?!??" +
                                               $"Deadline sau khi sửa phải là ít nhất 1 ngày từ bây giờ ~";
                            return response;
                        }
                        actionDetails.Add($"Sửa hạn nộp thành {request.Deadline}");
                        targetTask.DeadLine = (DateTime)request.Deadline;
                        changed = true;
                        deadlineChanged = true;
                    }
                }
                if (!changed)
                {
                    response.Message = "Không có thay đổi ~";
                    response.StatusCode = System.Net.HttpStatusCode.NoContent;
                    response.IsSuccess = true;
                    return response;
                }
                else
                {
                    targetTask.UpdatedDate = currentTime;
                    //targetTask.UpdatedBy = requestedUser;
                    targetTask.Id = request.RequestedUserId;
                }

                // Save changes to database
                if (deadlineChanged)
                {
                    // Đoạn này để un-schedule background task cũ: gửi email inform khi sắp đến deadline nếu task đó
                    // chưa được thực thi
                    // và schedule một background task mới
                    if (targetTask.ScheduleTaskId != null)
                        await Task.Run(() => _unitOfWork.DeleteScheduledJob(targetTask.ScheduleTaskId));
                    targetTask.ScheduleTaskId = await AutoSendReminderEmailBeforeDeadLine(new RemindTaskDeadlineRequest
                    {
                        AssigneeEmail = newAssignee.Email,
                        AssigneeName = newAssignee.UserName,
                        Deadline = targetTask.DeadLine,
                        TaskDetails = newTaskDetails,
                        TaskName = targetTask.TaskName,
                        TaskId = targetTask.Id,
                    });
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.AccessLogs.AddAsync(new()
                {
                    IsSuccess = true,
                    AccessTime = currentTime,
                    ActionName = $"Chỉnh sửa nhiệm vụ {targetTask.TaskName}",
                    IpAddress = request.IpAddress ?? "::1",
                    UserId = request.RequestedUserId,
                    TargetObject = nameof(AddQuestionTask),
                    Details = string.Join(" & ", actionDetails)
                });

                newTaskDetails = string.Format(
                    targetTask.ForMultiChoice ? SystemDataFormat.TASK_MULTIPLECHOCIE_INFO_FORMAT : SystemDataFormat.TASK_ESSAY_INFO_FORMAT,
                    targetTask.TaskName,
                    targetTask.DeadLine.ToString("yyyy-MM-dd"),
                    levelDetail.Topic.TopicName,
                    targetTask.Difficult.ToString(),
                    targetTask.NumberOfQuestion,
                    questionType.Skill.ToString(),
                    questionType.TypeName
                );
                if (targetTask.AssignId == request.AssigneeId)
                {
                    string emailBody = string.Format(SystemDataFormat.INFORM_UPDATETASK_EMAIL_BODY,
                                 newAssignee.UserName,
                                 requestedUser.UserName,
                                 targetTask.TaskName,
                                 oldTaskDetails,
                                 newTaskDetails);
                    //_unitOfWork.EmailService.SendEmail(newAssignee.Email,
                    //            SystemDataFormat.INFORM_UPDATETASK_EMAIL_SUBJECT,
                    //            emailBody,
                    //            newAssignee.UserName);
                }
                else
                {
                    string toOldAssigneeEmailBody = string.Format(SystemDataFormat.INFORM_REMOVEACCESS_FROMTASK_EMAIL_BODY,
                                 oldAssignee.UserName,
                                 requestedUser.UserName,
                                 targetTask.TaskName,
                                 oldAssignee.UserName);
                    string toNewAssigneeEmailBody = string.Format(SystemDataFormat.INFORM_NEWTASK_EMAIL_BODY,
                                 newAssignee.UserName,
                                 requestedUser.UserName,
                                 1,
                                 newTaskDetails);
                    //_unitOfWork.EmailService.SendEmail(oldAssignee.Email,
                    //            SystemDataFormat.INFORM_REMOVEACCESS_FROMTASK_EMAIL_BODY,
                    //            toOldAssigneeEmailBody,
                    //            oldAssignee.UserName);
                    //_unitOfWork.EmailService.SendEmail(newAssignee.Email,
                    //            SystemDataFormat.INFORM_NEWTASK_EMAIL_BODY,
                    //            toNewAssigneeEmailBody,
                    //            newAssignee.UserName);
                }

                response.Result.Add(_mapper.Map<TaskGeneralDisplayDTO>(targetTask));
                //response.Message = string.Join(" & ", actionDetails);
                response.Message = AppMessages.TASK_UPDATE_SUCCESS;
                response.StatusCode = System.Net.HttpStatusCode.OK;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return response;
        }
        public async Task<APIResponse<TaskGeneralDisplayDTO>> GetImportQuestionTask(ViewTaskRequest request)
        {
            APIResponse<TaskGeneralDisplayDTO> response = new();
            Expression<Func<AddQuestionTask, bool>> filter = x => true; // Init filter ~
            // Combine all filters ~
            {
                if (request.AssigneeId != null)
                {
                    Expression<Func<AddQuestionTask, bool>> tempFilter = x => x.AssignId == request.AssigneeId;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
                if (request.LevelId != null)
                {
                    Expression<Func<AddQuestionTask, bool>> tempFilter = x => x.LevelDetail.LevelId == request.LevelId;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
                if (request.QuestionTypeId != null)
                {
                    Expression<Func<AddQuestionTask, bool>> tempFilter = x => x.QuestionTypeId == request.QuestionTypeId;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
                if (request.Status != null)
                {
                    Expression<Func<AddQuestionTask, bool>> tempFilter = x => x.Status == request.Status;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
                if (request.LowerBound != null)
                {
                    Expression<Func<AddQuestionTask, bool>> tempFilter = x => x.CreatedDate >= request.LowerBound;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
                if (request.UpperBound != null)
                {
                    Expression<Func<AddQuestionTask, bool>> tempFilter = x => x.CreatedDate <= request.UpperBound;
                    filter = ExpressionExtension.CombineFilters(filter, tempFilter);
                }
            }
            try
            {
                var total = await _unitOfWork.AddQuestionTasks.CountAsync(filter);
                var queriedResult = await _unitOfWork.AddQuestionTasks.GetAllAsync(
                    filter: filter,
                    includeProperties: "User,Questions,QuestionType,Questions.QuestionType,LevelDetail,LevelDetail.Topic,LevelDetail.Level",
                    orderBy: x => x.OrderByDescending(obj => obj.CreatedDate),
                    pageNumber: request.PaginationRequest?.Page ?? 0,
                    pageSize: request.PaginationRequest?.Size ?? 0);

                var count = queriedResult.Count;
                if (count > 0) // Map data if existing
                {
                    var finalResult = _mapper.Map<List<TaskGeneralDisplayDTO>>(queriedResult.ToList());
                    response.Result = finalResult;
                    response.TotalCount = total;
                    response.Pagination = new Pagination
                    {
                        Page = request.PaginationRequest.Page,
                        Size = request.PaginationRequest.Size
                    };
                }
                else // else return no content
                {
                    response.StatusCode = System.Net.HttpStatusCode.NoContent;
                    response.Message = AppMessages.NO_CONTENT;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;
        }

        public async Task<APIResponse<TaskFullDisplayDTO>> GetTaskDetail(Guid id)
        {
            APIResponse<TaskFullDisplayDTO> response = new();

            try
            {
                var task = await _unitOfWork.AddQuestionTasks.GetAsync(x => x.Id == id,
                            includeProperties: "User,QuestionType,Questions,LevelDetail,LevelDetail.Topic,LevelDetail.Level");
                if (task == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                    return response;
                }
                var questions = await _unitOfWork.Questions.GetAllAsync(x => x.TaskId == id,
                                includeProperties: "QuestionType,LevelDetail,LevelDetail.Topic,LevelDetail.Level");
                var historyIds = questions.Select(x => x.LogId).Distinct().ToList();
                var histories = await _unitOfWork.AccessLogs.GetAllAsync(x => historyIds.Contains(x.Id));
                var finalResult = _mapper.Map<TaskFullDisplayDTO>(task);
                if (questions.Count > 0)
                {
                    var groupedQues = questions.GroupBy(x => new { x.LevelDetail, x.QuestionType, x.IsMultipleChoice, x.Difficulty });
                    foreach (var group in groupedQues)
                    {
                        SumUpImportedQuestionsDTO sum = new()
                        {
                            LevelDetail = $"{group.Key.LevelDetail.Level.LevelName} - {group.Key.LevelDetail.Topic.TopicName}",
                            QuestionTypeName = group.Key.QuestionType.TypeName,
                            QuestionForm = group.Key.IsMultipleChoice ? "Trắc nghiệm" : "Tự luận",
                            Difficulty = group.Key.Difficulty.ToString(),
                            TotalQuestions = group.Count(),
                            ApprovedQuestions = group.Count(x => x.Status == QuestionStatus.Ok),
                            Skill = group.Key.QuestionType.Skill.ToString(),
                        };
                        finalResult.SumUp.Add(sum);
                    }
                }
                foreach (var item in histories)
                {
                    AddQuestionHistoryDto newDTO = new()
                    {
                        ActionName = item.ActionName,
                        UserName = item.User?.UserName ?? "",
                        AccessTime = item.AccessTime,
                        Id = item.Id,
                        TotalQuestions = questions.Count(x => x.LogId == item.Id),
                        ApprovedQuestions = questions.Count(x => x.LogId == item.Id && x.Status == Domain.Enums.QuestionStatus.Ok),
                        TaskName = task.TaskName,
                    };
                    finalResult.ImportHistory.Add(newDTO);
                }
                response.Result.Add(finalResult);
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }

            return response;
        }
    }
}
