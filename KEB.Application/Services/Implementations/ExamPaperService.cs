using AutoMapper;
using KEB.Application.DTOs.ExamPaperDTO;
using KEB.Application.DTOs.ExamTypeConstraintDTO;
using KEB.Application.DTOs.SystemAccessLogDTO;
using KEB.Application.Services.Interfaces;
using KEB.Application.Utils;
using KEB.Domain.Entities;
using KEB.Domain.Enums;
using KEB.Domain.ValueObject;
using KEB.Infrastructure.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static KEB.Domain.ValueObject.LogicString;

namespace KEB.Application.Services.Implementations
{
    public class ExamPaperService : IExamPaperService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ExamPaperService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponse<PaperGeneralDisplayDTO>> ChangePaperStatus(ChangePaperStatusRequest request)

        {
            APIResponse<PaperGeneralDisplayDTO> response = new() { IsSuccess = false };
            try
            {
                var paper = await _unitOfWork.Papers.GetAsync(x => x.Id == request.PaperId,
                                        includeProperties: "Exam,Exam.Reviewer,Exam.Reviewer.Role,Exam.Host,Exam.Host.Role");
                if (paper == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                    return response;
                }
                var exam = paper.Exam;
                var user = await _unitOfWork.Users.GetAsync(x => x.Id == request.RequestedUserId, includeProperties: "Role");
                bool isAuthorize = true;
                if (user == null)
                {
                    isAuthorize = false;
                }
                else
                {
                    if (paper.PaperStatus == request.NewStatus)
                    {
                        response.Message = "Không có thay đổi";
                        response.StatusCode = System.Net.HttpStatusCode.NoContent;
                        return response;
                    }
                    bool paperIsDone = paper.PaperStatus == PaperStatus.Done;
                    bool hostCanAct = exam.HostId == request.RequestedUserId && request.NewStatus == PaperStatus.Done && !paperIsDone;
                    bool reviewerCanAct = exam.ReviewerId == request.RequestedUserId && request.NewStatus != PaperStatus.Done && !paperIsDone;
                    isAuthorize = !hostCanAct && !reviewerCanAct;
                }
                if (!isAuthorize)
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.Message = "Không đủ quyền hạn để sưas";
                    return response;
                }
                await _unitOfWork.BeginTransactionAsync();
                paper.PaperStatus = request.NewStatus;
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.AccessLogs.AddAsync(new()
                {
                    AccessTime = DateTime.Now,
                    ActionName = "Đổi status",
                    IpAddress = request.IpAddress ?? "::1",
                    IsSuccess = true,
                    TargetObject = $"{paper.PaperName}",
                    UserId = request.RequestedUserId,
                    Details = $"{user.UserName} chuyển đề thi {paper.PaperName} từ status {paper.PaperStatus} sang status {request.NewStatus}"
                });
                paper.PaperStatus = request.NewStatus;
                response.IsSuccess = true;
                response.Result.Add(_mapper.Map<PaperGeneralDisplayDTO>(paper));
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<APIResponse<PaperGeneralDisplayDTO>> EditExamPaper(EditPaperDetailRequest request)

        {
            APIResponse<PaperGeneralDisplayDTO> response = new() { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };
            Paper? paper = null;
            Exam? exam = null;
            ExamType? examType = null;
            ExamTypeConstraint? constraint = null;
            DateTime currentTime = DateTime.Now;
            User? requestedUser = null;
            List<Question> newQuestions = [];
            try
            {
                paper = await _unitOfWork.Papers.GetAsync(x => x.Id == request.PaperId,
                               includeProperties: "Exam,Exam.Reviewer,Exam.Reviewer.Role," +
                                                  "Exam.Host,Exam.Host.Role," +
                                                  "PaperQuestions,PaperQuestions.Question");
                if (paper == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                    return response;
                }
                else
                {
                    if (paper.Skill == Skill.Nghe) throw new NotImplementedException("This feature hasn't been implemented yet ~ ");
                }
                exam = paper.Exam;
                if (exam.CreatedDate.AddDays(SystemDataFormat.EXAM_INFO_EDIT_DURATION + SystemDataFormat.EXAM_PAPERS_EDIT_DURATION) < currentTime)
                    throw new UnauthorizedAccessException("Papers can only be edited within the editing time (within 2 days from exam finish creating)");

                examType = await _unitOfWork.ExamTypes.GetAsync(x => x.Id == exam.ExamTypeId, includeProperties: "Level");
                if (examType == null)
                    throw new ArgumentNullException("Exam type not found ~");

                constraint = await _unitOfWork.ExamTypesConstraints.GetAsync(x => x.ExamTypeId == exam.ExamTypeId && x.Skill == paper.Skill,
                                                            includeProperties: "ConstraintDetails," +
                                                                               "ConstraintDetails.QuestionType," +
                                                                               "ConstraintDetails.Topic");
                if (constraint == null)
                    throw new ArgumentNullException("Exam constraint not found ~");

                requestedUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.RequestedUserId, includeProperties: "Role");
                if (requestedUser == null || requestedUser.Id != exam.HostId || paper.PaperStatus == PaperStatus.Done)
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.Message = AppMessages.NO_PERMISSION;
                    return response;
                }
                var tmp = await _unitOfWork.Questions.GetAllAsync(filter: x => request.NewQuestions.Contains(x.Id),
                                                    includeProperties: "LevelDetail,LevelDetail.Topic,LevelDetail.Level,QuestionType");
                newQuestions = [.. tmp];
            }
            catch (UnauthorizedAccessException e)
            {
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.Message = e.Message;
                return response;
            }
            catch (NotImplementedException e)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotImplemented;
                response.Message = e.Message;
                return response;
            }
            catch (ArgumentNullException e)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = e.Message;
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                return response;
            }
            var (Result, IsSuccess) = MatchQuestionListWithConstraint([.. newQuestions], constraint, examType.Levels, request.PaperId);
            List<string> changedDetails = [];
            if (IsSuccess)
            {
                try
                {
                    await _unitOfWork.BeginTransactionAsync();

                    paper.PaperDetails = [];
                    foreach (var item in Result)
                    {
                        paper.PaperDetails.Add((PaperDetail)item);
                    }
                    await _unitOfWork.AccessLogs.AddAsync(new()
                    {
                        AccessTime = currentTime,
                        ActionName = "Sửa đề thi",
                        IpAddress = request.IpAddress ?? "::1",
                        IsSuccess = true,
                        TargetObject = $"{paper.PaperName}",
                        UserId = request.RequestedUserId,
                        Details = $"{requestedUser.UserName} sửa nội dung đề thi"
                    });
                    await Console.Out.WriteLineAsync("Edit exam paper run");
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    response.IsSuccess = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Message = "Edit paper successfully";
                    response.Result.Add(_mapper.Map<PaperGeneralDisplayDTO>(paper));
                    return response;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                    response.Message = ex.Message;
                    response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                    return response;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = string.Join("<br/>", Result);
                return response;
            }
        }
        private static (IEnumerable<object> Result, bool IsSuccess) MatchQuestionListWithConstraint(List<Question> questions, ExamTypeConstraint constraints, Level level, Guid paperId)
        {
            List<string> errorMessages = [];
            List<PaperDetail> result = [];
            foreach (var detail in constraints.ConstraintDetails)
            {
                var tmpQues = questions.Where(x => true
                                             && x.LevelDetail.LevelId == level.Id
                                             && x.IsDeleted == false
                                             //&& x.Status == QuestionStatus.Ok
                                             && x.Difficulty == detail.Difficulty
                                             && x.IsMultipleChoice == detail.IsMultipleChoice
                                             && x.QuestionTypeId == detail.QuestionTypeId
                                             && x.LevelDetail.TopicId == detail.TopicId);
                int count = tmpQues.Count();
                if (count == detail.NumberOfQuestion)
                {
                    result.AddRange(tmpQues.Select(x => new PaperDetail
                    {
                        Mark = detail.MarkPerQuestion,
                        PaperId = paperId,
                        Question = x,
                        QuestionId = x.Id,
                    }).ToList());
                }
                else if (count > detail.NumberOfQuestion)
                {
                    errorMessages.Add($"Có nhiều hơn {detail.NumberOfQuestion} câu hỏi " +
                                       $"{(detail.IsMultipleChoice ? "Trắc Nghiệm" : "Tự luận")} {detail.Difficulty} " +
                                       $"mang chủ đề {detail.Topic.TopicName}-{level.LevelName} " +
                                       $"thuộc loại câu hỏi {detail.QuestionType.TypeName}");
                }
                else errorMessages.Add($"Không đủ {detail.NumberOfQuestion} câu hỏi " +
                                       $"{(detail.IsMultipleChoice ? "Trắc Nghiệm" : "Tự luận")} {detail.Difficulty} " +
                                       $"mang chủ đề {detail.Topic.TopicName}-{level.LevelName} " +
                                       $"thuộc loại câu hỏi {detail.QuestionType.TypeName}");
                questions.RemoveAll(x => true
                                             && x.IsDeleted == false
                                             && x.Difficulty == detail.Difficulty
                                             && x.IsMultipleChoice == detail.IsMultipleChoice
                                             && x.QuestionTypeId == detail.QuestionTypeId
                                             && x.LevelDetail.LevelId == level.Id
                                             && x.LevelDetail.TopicId == detail.TopicId);
            }
            if (questions.Count > 0) errorMessages.Add($"Thừa {questions.Count} câu hỏi rồi ~");
            if (errorMessages.Count > 0) return (errorMessages.Select(x => (object)x), false);
            else return (result.Select(x => (object)x), true);
        }
        public Task<APIResponse<string>> GetUrlOfPaperAudio(ViewPaperDetailRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<APIResponse<string>> GetUrlOfPaperContentPdf(ViewPaperDetailRequest request)
        {
            throw new NotImplementedException();
            //APIResponse<string> response = new();
            //try
            //{
            //    bool isAuthorized = true;
            //    Paper? paper = null;
            //    Exam? exam = null;
            //    DateTime currentTime = DateTime.Now;
            //    var requestedUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.RequestedUserId);
            //    if (requestedUser == null)
            //        throw new UnauthorizedAccessException(AppMessages.NO_PERMISSION);
            //    else
            //    {
            //        paper = await _unitOfWork.Papers.GetAsync(x => x.Id == request.PaperId,
            //        includeProperties: "Exam,Exam.ExamType," +
            //                           "Exam.Host,Exam.Reviewer," +
            //                           "Exam.ExamType.Levels," +
            //                           "PaperDetails,PaperDetails.Question," +
            //                           "PaperDetails.Question.LevelDetail," +
            //                           "PaperDetails.Question.LevelDetail.Level," +
            //                           "PaperDetails.Question.LevelDetail.Topic," +
            //                           "PaperDetails.Question.References," +
            //                           "PaperDetails.Question.Answers," +
            //                           "PaperDetails.Question.QuestionType"
            //                        );
            //        if (paper == null)
            //        {
            //            response.StatusCode = System.Net.HttpStatusCode.NotFound;
            //            response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
            //            return response;
            //        }

            //        exam = paper.Exam;
            //        bool examHidden = exam.IsDeleted; // this means the exam is still visible but the papers is not
            //        bool examSuspended = exam.IsSuspended; // this means the exam is canceled or suspended so that it actually did not/will not take place
            //        bool examInEdit = exam.CreatedDate.AddDays(3) > currentTime; // 1 day for team lead to edit exam info & 2 day to edit & review exam paper
            //        bool examInPrepare = currentTime > exam.TakePlaceTime.AddDays(-1) && currentTime < exam.TakePlaceTime; // 24 hours before exam taking place
            //        bool examTookPlace = currentTime > exam.TakePlaceTime;
            //        bool isTeamLead = requestedUser.RoleId.ToString() == LogicString.Role.TeamLeadRoleId;
            //        bool review = exam.ReviewerId == request.RequestedUserId;
            //        bool host = exam.HostId == request.RequestedUserId;

            //        if (examTookPlace || examSuspended)
            //        {
            //            isAuthorized = examHidden;
                        
            //        }
            //        else
            //        {
            //            // If the exam hasn't taken place & is not suspended
            //            if (examInEdit || examInPrepare)
            //            {
            //                isAuthorized = review || host || isTeamLead;
            //            }
            //            else
            //            {
            //                isAuthorized = false;
            //            }
            //        }
            //    }
            //    if (!isAuthorized) throw new UnauthorizedAccessException("You are not allowed to download the paper at this time");
            //    else
            //    {
            //        string filePath = string.IsNullOrEmpty(paper.ExportedContentUrl) ? await AutoGenerateAndSetPaperContentPdf(paper) : paper.ExportedContentUrl;

            //        // Nếu đường dẫn bắt đầu bằng "/GeneratedPapers/", chuyển thành đường dẫn đầy đủ
            //        if (filePath.StartsWith("/GeneratedPapers/"))
            //        {
            //            filePath = Path.Combine(Environment.CurrentDirectory, filePath.TrimStart('/'));
            //        }

            //        response.Result.Add(filePath);
            //    }
            //}
            //catch (UnauthorizedAccessException e)
            //{
            //    response.IsSuccess = false;
            //    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
            //    response.Message = e.Message;
            //}
            //catch (Exception e)
            //{
            //    response.IsSuccess = false;
            //    response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            //    response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            //}
            //return response;
        }
        private async Task<string> AutoGenerateAndSetPaperContentPdf(Paper paper)
        {
            Exam exam = paper.Exam;
            DateTime currentTime = DateTime.Now;
            var allQuestions = paper.PaperDetails.OrderBy(x => x.OrderInPaper).Select(x => x.Question).ToList();
            string templatePath = Path.Combine(Environment.CurrentDirectory, "ExternalFiles", "index.html");
            string logoPath = AzureBlob.SYSTEM_LOGO_URL;
            StringBuilder questionsHtml = new();

            if (paper.Skill == Skill.Nghe)
            {
                int quesIndex = 1;
                foreach (var question in allQuestions)
                {
                    questionsHtml.Append($@"
                                    <div class='question-title'>
                                        <p>{question.QuestionType.TypeName}</p>
                                    </div>");
                    var tmpQuesContent = GetQuestionHtml(question, quesIndex);
                    questionsHtml.Append(tmpQuesContent);
                    quesIndex++;
                }
            }
            else
            {
                var groupedQuestions = allQuestions.GroupBy(x => x.QuestionType);
                int typeIndex = 1;
                int quesIndex = 1;
                foreach (var item in groupedQuestions)
                {
                    questionsHtml.Append($@"
                                    <div class='question-title'>
                                        <p>{CommonUntils.ConvertIntegerToRoman(typeIndex)}. {item.Key.TypeName}</p>
                                    </div>");
                    foreach (var question in item)
                    {
                        var tmpQuesContent = GetQuestionHtml(question, quesIndex);
                        questionsHtml.Append(tmpQuesContent);
                        quesIndex++;
                    }
                    typeIndex++;
                }
            }

            string htmlContent = File.ReadAllText(templatePath);
            htmlContent = htmlContent
                .Replace("{ExamName}", exam.ExamName)
                .Replace("{LogoPath}", logoPath)
                .Replace("{ExamDate}", exam.TakePlaceTime.ToString("dd MMM yyyy"))
                .Replace("{PaperName}", paper.PaperName)
                .Replace("{Questions}", questionsHtml.ToString());

            var renderer = new ChromePdfRenderer();
            var pdfFromHtmlFile = renderer.RenderHtmlAsPdf(htmlContent);
            // Tạo thư mục để lưu các file PDF nếu nó chưa tồn tại
            string pdfDirectory = Path.Combine(Environment.CurrentDirectory, "GeneratedPapers");
            if (!Directory.Exists(pdfDirectory))
            {
                Directory.CreateDirectory(pdfDirectory);
            }
            // Tạo tên file dựa trên thông tin paper
            string fileName = $"{paper.PaperName}_{paper.Skill}_{currentTime:yyyyMMdd_HHmmss}.pdf";
            string filePath = Path.Combine(pdfDirectory, fileName);

            // Lưu PDF vào file
            pdfFromHtmlFile.SaveAs(filePath);
            // Lưu URL tương đối của file trong database
            //paper.ExportedContentUrl = $"/GeneratedPapers/{fileName}";
            await _unitOfWork.SaveChangesAsync();
            // Log hoạt động
            await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
            {
                AccessTime = currentTime,
                ActionName = "Generate pdf for exam paper",
                IpAddress = "",
                IsSuccess = true,
                TargetObject = $"{paper.PaperName}",
                Details = $"Generate pdf for paper {paper.PaperName} and save to local path {filePath}"
            });

            return filePath;
        }
        private static string GetQuestionHtml(Question question, int quesIndex)
        {
            StringBuilder tmpAnswersContent = new();
            bool useFlex = false;
            int answerIndex = 1;
            foreach (var answer in question.Answers)
            {
                if (answer.AnswerContent.Length > 48) useFlex = true;
                tmpAnswersContent.Append(string.Format(SystemDataFormat.SINGLE_ANSWER_HTML_FORMAT,
                    CommonUntils.ConvertIntegerToLetter(answerIndex),
                    answer.AnswerContent));
                answerIndex++;
            }

            // Xử lý tệp đính kèm nếu cần
            //string attachmentHtml = "";
            //if (!string.IsNullOrEmpty(question.AttachmentFile) && question.QuestionType.Skill != Skill.Nghe)
            //{
            //    var url = question.AttachmentFileName;
            //    attachmentHtml = \$"\<img style="width:36%"" +
            //                \$"src="{url}" alt="Attachment for question could not be loaded">";
            //}

            string? tmpQuesContent;
            if (useFlex)
                tmpQuesContent = string.Format(SystemDataFormat.QUESTION_FLEXANSWER_HTML_FORMAT,
                    quesIndex,
                    question.QuestionContent,
                    2,
                    // attachmentHtml,
                    tmpAnswersContent.ToString());
            else
                tmpQuesContent = string.Format(SystemDataFormat.QUESTION_GRIDANSWER_HTML_FORMAT,
                    quesIndex,
                    question.QuestionContent,
                    2,
                    // attachmentHtml,
                    tmpAnswersContent.ToString());

            return tmpQuesContent;
        }
        public async Task<APIResponse<object>> LeaveCommentOnPaper(LeaveCommentRequest request)
        {
            APIResponse<object> response = new() { IsSuccess = false };
            List<string> validateResult = [];
            try
            {
                var requestedUser = await _unitOfWork.Users.GetByIdAsync(request.RequestedUserId);
                if (requestedUser == null)
                {
                    validateResult.Add(LogicString.Permission.NoPermission);
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                }
                if (string.IsNullOrEmpty(request.Content))
                {
                    validateResult.Add("Nhận xét mà 0 có nội dung thì để làm gì ???");
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                }
                var examPaper = await _unitOfWork.Papers.GetAsync(x => x.Id == request.TargetObjectId, includeProperties: "Exam");
                if (examPaper == null)
                {
                    validateResult.Add("Không tìm thấy dữ liệu");
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                }
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    response.Message = string.Join(" ~ ", validateResult);
                    return response;
                }
                var comment = new SystemAccessLog
                {
                    AccessTime = DateTime.Now,
                    ActionName = "Comment on exam paper",
                    UserId = request.RequestedUserId,
                    IpAddress = request.IpAddress ?? "::1",
                    IsSuccess = true,
                    Details = request.Content,
                    TargetObject = $"{examPaper.PaperName}",
                };
                await _unitOfWork.AccessLogs.AddAsync(comment);
                response.IsSuccess = true;
                //response.Result.Add(comment);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return response;
        }

        public Task<APIResponse<object>> UploadExamMaterials(Guid examId, Guid? requestedUserId = null, string ipAddress = "")
        {
            throw new NotImplementedException();
        }

        public async Task<APIResponse<PaperDetailDisplayDTO>> UserGenerateExamPapers(
     Guid examId, Guid requestedUserId, string? ipAddress)

        {
            APIResponse<PaperDetailDisplayDTO> response = new();
            Exam? exam = null;
            ExamType? examType = null;
            User? requestedUser = null;
            DateTime currentTime = DateTime.Now;
            try
            {
                exam = await _unitOfWork.Exams.GetAsync(x => x.Id == examId,
                                                    includeProperties: "Host,Reviewer,ExamType,Papers")
                        ?? throw new InvalidOperationException("Exam not found ~");
                //bool inEditingTime = currentTime > exam.CreatedDate.AddDays(SystemDataFormat.EXAM_INFO_EDIT_DURATION)
                //        && currentTime < exam.CreatedDate.AddDays(SystemDataFormat.EXAM_INFO_EDIT_DURATION + SystemDataFormat.EXAM_PAPERS_EDIT_DURATION);
                //if (!inEditingTime)
                //{
                //    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                //    response.Message = "Papers can only be generated within the editing time (within 2 days from exam finish creating)";
                //    response.IsSuccess = false;
                //    return response;
                //}
                bool isAuthorized = false;
                if (requestedUserId == exam.HostId || requestedUserId == exam.ReviewerId)
                {
                    requestedUser = await _unitOfWork.Users.GetAsync(x => x.Id == requestedUserId);
                    if (requestedUser == null || requestedUser.IsActive == false)
                    {
                        isAuthorized = false;
                    }
                    else isAuthorized = true;
                }
                if (!isAuthorized)
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.Message = AppMessages.NO_PERMISSION;
                    response.IsSuccess = false;
                    return response;
                }

                examType = await _unitOfWork.ExamTypes.GetAsync(x => x.Id == exam.ExamTypeId,
                        includeProperties: "Levels,Levels.LevelDetails," +
                                           "ExamTypeConstraints,ExamTypeConstraints.ConstraintDetails," +
                                           "ExamTypeConstraints.ConstraintDetails.QuestionType," +
                                           "ExamTypeConstraints.ConstraintDetails.Topic"
                        ) ?? throw new InvalidOperationException("Exam type not found ~");

                var questionsPool = await GetQuestionPoolOfExamType(examType);
                var constraints = examType.ExamTypeConstraints;
                // Start
                await _unitOfWork.BeginTransactionAsync();
                // Log the generate papers request
                await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                {
                    AccessTime = currentTime,
                    ActionName = "Yêu cầu tạo đề thi",
                    Details = exam.Papers.Count > 0 ? $"Bỏ {exam.Papers.Count} đề thi cũ và tạo lại đề mới ~" : "Tự động tạo đề thi mới ~",
                    IpAddress = ipAddress ?? "",
                    IsSuccess = true,
                    TargetObject = $"{exam.ExamName}",
                    UserId = requestedUserId,
                });
                await _unitOfWork.Papers.DeleteRangeAsync(exam.Papers);
                await _unitOfWork.SaveChangesAsync();
                foreach (var constraint in constraints)
                {
                    var skill = constraint.Skill;
                    var result = await GenerateExamPaperFollowConstraint(requestedUserId, constraint, exam, examType.LevelId, questionsPool);
                    foreach (var item in result)
                    {
                        response.Result.Add(item);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
                // End
                await _unitOfWork.CommitAsync();
            }
            catch (InvalidOperationException e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
                response.StatusCode = System.Net.HttpStatusCode.Conflict;
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.Message);
                await _unitOfWork.RollbackAsync();
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
                response.IsSuccess = false;
            }
            return response;
        }

        private async Task<List<Question>> GetQuestionPoolOfExamType(ExamType examType)
        {
           
            var constraints = examType.ExamTypeConstraints;

            Expression<Func<Question, bool>> filter = x => true;
            List<Guid> qtypes = [];
            List<Guid> topics = [];
            List<Difficulty> difficulties = [];
            bool allMultipleChoice = true;
            bool noMultipleChoice = false;
            foreach (var constraint in constraints)
            {
                foreach (var detail in constraint.ConstraintDetails)
                {
                    qtypes.Add(detail.QuestionTypeId);
                    topics.Add(detail.TopicId);
                    difficulties.Add(detail.Difficulty);
                }
                allMultipleChoice = constraint.ConstraintDetails.All(x => x.IsMultipleChoice);
                noMultipleChoice = constraint.ConstraintDetails.All(y => !y.IsMultipleChoice);
            }
            qtypes = qtypes.Distinct().ToList();
            topics = topics.Distinct().ToList();
            difficulties = difficulties.Distinct().ToList();

            var result = await _unitOfWork.Questions.GetAllAsync(
                filter: x => true
                        && x.LevelDetail.LevelId == examType.LevelId
                        && topics.Contains(x.LevelDetail.TopicId)
                        && qtypes.Contains(x.QuestionTypeId)
                        && (!allMultipleChoice || x.IsMultipleChoice)
                        && (!noMultipleChoice || !x.IsMultipleChoice),
                includeProperties: "References,Answers");

            return [.. result];
        }
        private async Task<List<PaperDetailDisplayDTO>> GenerateExamPaperFollowConstraint(Guid requestedUserId , ExamTypeConstraint constraint, Exam exam, Guid levelId, List<Question> questionsPool)
        {
            
            List<PaperDetailDisplayDTO> result = [];
            DateTime currentTime = DateTime.Now;
            var skill = constraint.Skill;
            if (skill != Skill.Nghe)
            {
                int time = constraint.NumberOfPaper;
                while (time > 0)
                {
                    Paper newPaper = new()
                    {
                        Id = Guid.NewGuid(),
                        CreatedBy = constraint.CreatedBy,
                        UpdatedBy = constraint.UpdatedBy,
                        PaperName = $"{exam.ExamName}" +
                                    $"_{DateTime.Now:yyyy MMM dd}" +
                                    $"_{skill}_No{time}",
                        CreatedDate = DateTime.Now,
                        ExamId = exam.Id,
                        Skill = constraint.Skill,
                        PaperStatus = PaperStatus.Creating,
                        PaperDetails = [],
                        AttachmentUrl= "",
                        PaperFileUrl = "",
                        IsReviewed = false,
                        IsDeleted = false,
                    };
                    time--;
                    
                    await _unitOfWork.Papers.AddAsync(newPaper);
                    PaperDetailDisplayDTO mappedPaper = _mapper.Map<PaperDetailDisplayDTO>(newPaper);
                    foreach (var detail in constraint.ConstraintDetails)
                    {
                        GetRandomListRequest randomRequest = new()
                        {
                            Skill = skill,
                            ConstraintDetail = detail,
                            ExamId = exam.Id,
                            LevelId = levelId
                        };
                        List<Question> randomResultOfConstraint = ApplyRandomPickingAlgorithms(randomRequest, questionsPool);

                        foreach (var question in randomResultOfConstraint)
                        {

                            newPaper.PaperDetails.Add(new PaperDetail
                            {
                                QuestionId = question.Id,
                                //Question = question,
                                PaperId = newPaper.Id,
                                Mark = detail.MarkPerQuestion,

                            });
                            var mappedQuestion = _mapper.Map<QuestionInPaperDTO>(question);
                            mappedQuestion.Mark = detail.MarkPerQuestion;
                            mappedPaper.QuestionsList.Add(mappedQuestion);
                        }
                    }
                    await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                    {
                        AccessTime = currentTime,
                        ActionName = "Tự động tạo đề thi",
                        IpAddress = "",
                        IsSuccess = true,
                        TargetObject = $"{newPaper.PaperName}",
                        Details = "",
                        UserId = requestedUserId
                    });
                    mappedPaper.PaperConstraint = _mapper.Map<ConstraintToBeDisplayedDTO>(constraint);
                    result.Add(mappedPaper);
                }
            }
            else
            {
                List<(Paper, HashSet<Guid>)> newPapers = [];
                Random rand = new();
                for (int i = 0; i < constraint.NumberOfPaper; i++)
                {
                    var newPaper = new Paper
                    {
                        PaperName = $"{exam.ExamName}" +
                                    $"_{currentTime:yyyy MMM dd}" +
                                    $"_{skill}_No{i + 1}",
                        CreatedDate = currentTime,
                        ExamId = exam.Id,
                        Skill = constraint.Skill,
                        PaperStatus = PaperStatus.Creating,
                        PaperDetails = [],
                        IsReviewed = false,
                        Exam = exam,
                    };
                    newPapers.Add((newPaper, []));
                    //await _unitOfWork.Papers.AddAsync(newPaper);
                }
                await _unitOfWork.Papers.AddRangeAsync(newPapers.Select(x => x.Item1));

                //int currentOrderInPaper = 1;
                foreach (var detail in constraint.ConstraintDetails)
                {
                    var tempPool = questionsPool.Where(x => true
                                            //&& x.Status == QuestionStatus.Ok
                                            && x.LevelDetail.LevelId == levelId
                                            && x.QuestionType.Skill == skill
                                            && x.QuestionTypeId == detail.QuestionTypeId
                                            && x.LevelDetail.TopicId == detail.TopicId
                                            && x.Difficulty == detail.Difficulty
                                            && x.IsMultipleChoice == detail.IsMultipleChoice).ToList();
                    if (tempPool.Count < detail.NumberOfQuestion)
                    {
                        throw new InvalidOperationException("Không đủ câu hỏi");
                    }

                    var groupedByFileNamePool = tempPool.GroupBy(x => x.AttachmentFile).OrderBy(x => rand.Next());
                    int step = 0;
                    while (step < detail.NumberOfQuestion)
                    {
                        int index = rand.Next(groupedByFileNamePool.Count());
                        var itemGroup = groupedByFileNamePool.ElementAt(index);
                        var includedAll = true;
                        foreach (var ques in itemGroup)
                        {
                            if (!newPapers[0].Item2.Contains(ques.Id))
                            {
                                includedAll = false;
                                break;
                            }
                        }
                        if (!includedAll)
                        {
                            foreach (var item in newPapers)
                            {
                                Paper newPaper = item.Item1;
                                var continueLoop = true;
                                while (continueLoop/* && !includedAll*/)
                                {
                                    var grpCount = itemGroup.Count();
                                    var tmpQuestion = itemGroup.ElementAt(rand.Next(grpCount));
                                    if (item.Item2.Add(tmpQuestion.Id))
                                    {
                                        var tmpPaperQuestion = new PaperDetail
                                        {
                                            QuestionId = tmpQuestion.Id,
                                            PaperId = newPaper.Id,
                                            Question = tmpQuestion,
                                            Paper = newPaper,
                                            Mark = detail.MarkPerQuestion,
                                            //Attachment = itemGroup.Key,
                                            //OrderInPaper = currentOrderInPaper,
                                        };
                                        newPaper.PaperDetails.Add(tmpPaperQuestion);
                                        continueLoop = false;
                                    }
                                }
                            }
                            step++;
                            //currentOrderInPaper++;
                        }
                    }
                }
                foreach (var newPaper in newPapers)
                {
                    newPaper.Item1.PaperDetails = newPaper.Item1.PaperDetails.OrderBy(x => x.Attachment).Select((question, index) =>
                    {
                        question.OrderInPaper = index + 1;
                        return question;
                    }).ToList();
                    await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog
                    {
                        AccessTime = currentTime,
                        ActionName = "Tự động tạo đề thi",
                        IpAddress = "::1",
                        IsSuccess = true,
                        TargetObject = $"{newPaper.Item1.PaperName}",
                        Details = ""
                    });
                    PaperDetailDisplayDTO mappedPaper = _mapper.Map<PaperDetailDisplayDTO>(newPaper.Item1);
                    mappedPaper.PaperConstraint = _mapper.Map<ConstraintToBeDisplayedDTO>(constraint);
                    result.Add(mappedPaper);
                }
            }
            return result;
        }
        private static List<Question> ApplyRandomPickingAlgorithms(GetRandomListRequest request, List<Question> questionsPool)
        {
            List<Question> randomResultOfConstraint;
            var subQuestionsPool = questionsPool.Where(x => true
                //&& x.Status == QuestionStatus.Ok
                //&& x.LevelDetail?.LevelId == request.LevelId
                //&& x.QuestionType?.Skill == request.Skill
                && x.QuestionTypeId == request.ConstraintDetail.QuestionTypeId
                //&& x.LevelDetail?.TopicId == request.ConstraintDetail.TopicId
                && x.Difficulty == request.ConstraintDetail.Difficulty
                && x.IsMultipleChoice == request.ConstraintDetail.IsMultipleChoice).ToList();

            int randomNumber = new Random().Next(4);
            if (randomNumber == 0)
                randomResultOfConstraint = GetRandomListFollowConstraintDetailByRandomShuffling(request, subQuestionsPool);
            else if (randomNumber == 1)
                randomResultOfConstraint = GetRandomListFollowConstraintDetailByRandomIndexPicking(request, subQuestionsPool);
            else if (randomNumber == 2)
                randomResultOfConstraint = GetRandomListFollowConstraintDetailByRandomRemoving(request, subQuestionsPool);
            else
                randomResultOfConstraint = GetRandomListFollowConstraintDetailByReservoirSampler(request, subQuestionsPool);
            return randomResultOfConstraint;
        }
        private static List<Question> GetRandomListFollowConstraintDetailByRandomShuffling(GetRandomListRequest request, List<Question> questions)
        {
            var poolSize = questions.Count;
            int numOfQues = request.ConstraintDetail.NumberOfQuestion;
            if (poolSize < numOfQues) throw new InvalidOperationException("Ngân hàng không có đủ câu hỏi");
            Random rand = new();
            if (poolSize == 1) return questions.OrderBy(x => rand.Next()).ToList();

            for (int i = 0; i < numOfQues - 1; i++)
            {
                int j = rand.Next(i + 1);
                (questions[i], questions[j]) = (questions[j], questions[i]);
            }
            return questions.Take(numOfQues).ToList();
        }

        private static List<Question> GetRandomListFollowConstraintDetailByRandomRemoving(GetRandomListRequest request, List<Question> questions)
        {
            var poolSize = questions.Count;
            var numOfQues = request.ConstraintDetail.NumberOfQuestion;
            if (poolSize < numOfQues) throw new InvalidOperationException("Ngân hàng không có đủ câu hỏi");
            Random rand = new();
            if (poolSize == 1) return questions.OrderBy(x => rand.Next()).ToList();
            List<Question> result = [];
            List<Question> cloneList = new(questions);
            for (int i = 0; i < numOfQues; i++)
            {
                int randomIndex = rand.Next(0, cloneList.Count);
                result.Add(cloneList[randomIndex]);
                cloneList.RemoveAt(randomIndex);
            }
            return result;
        }

        private static List<Question> GetRandomListFollowConstraintDetailByRandomIndexPicking(GetRandomListRequest request, List<Question> questions)
        {
            var poolSize = questions.Count;
            var numOfQuest = request.ConstraintDetail.NumberOfQuestion;
            if (poolSize < numOfQuest) throw new InvalidOperationException("Ngân hàng không có đủ câu hỏi");
            Random random = new();
            if (poolSize == 1) return questions.OrderBy(x => random.Next()).ToList();
            List<Question> result = [];
            HashSet<int> selectedQuestions = [];
            while (selectedQuestions.Count < numOfQuest)
            {
                int index = random.Next(questions.Count);
                if (selectedQuestions.Add(index))
                {
                    result.Add(questions[index]);
                }
            }
            return result;
        }

        private static List<Question> GetRandomListFollowConstraintDetailByReservoirSampler(GetRandomListRequest request, List<Question> questions)
        {
            var poolSize = questions.Count;
            int numOfQues = request.ConstraintDetail.NumberOfQuestion;
            if (poolSize < numOfQues) throw new InvalidOperationException("Ngân hàng không có đủ câu hỏi");
            Random rand = new();
            if (poolSize == 1) return [.. questions.OrderBy(x => rand.Next())];

            List<Question> result = [];
            for (int i = 0; i < numOfQues; i++)
            {
                result.Add(questions[i]);
            }
            for (int i = numOfQues; i < poolSize; i++)
            {
                int randIndex = rand.Next(i + 1);
                if (randIndex < numOfQues)
                    result[randIndex] = questions[i];
            }
            return result;
        }
        public async Task<APIResponse<AccessLogDisplayDto>> ViewActivitiesOnPaper(Guid paperId)
        {
            APIResponse<AccessLogDisplayDto> response = new();
            try
            {
                var examPaper = await _unitOfWork.Papers.GetAsync(x => x.Id == paperId);
                if (examPaper == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                    response.IsSuccess = false;
                    return response;
                }
                var queried = await _unitOfWork.AccessLogs.GetAllAsync(x => x.TargetObject.Equals(examPaper.PaperName),
                                                            orderBy: x => x.OrderByDescending(x => x.AccessTime),
                                                            includeProperties: "User");


                List<AccessLogDisplayDto> result = [];
                string lastAction = "";
                foreach (var item in queried)
                {
                    if (lastAction.Equals("Xem đề thi") && item.ActionName.Equals("Xem đề thi"))
                    {
                        response.Result.RemoveAt(response.Result.Count - 1);
                        response.Result.Add(_mapper.Map<AccessLogDisplayDto>(item));
                        lastAction = "Xem đề thi";
                    }
                    else
                    {
                        lastAction = item.ActionName;
                        response.Result.Add(_mapper.Map<AccessLogDisplayDto>(item));
                    }
                }
                response.Message = $"{response.Result.Count}";
            }
            catch (Exception e)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = e.Message;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<APIResponse<PaperDetailDisplayDTO>> ViewExamPaperDetail(ViewPaperDetailRequest request)

        {
            APIResponse<PaperDetailDisplayDTO> response = new() { IsSuccess = false };
            try
            {
                bool isAuthorized = true;
                Paper? paper = null;
                DateTime currentTime = DateTime.Now;
                var requestedUser = await _unitOfWork.Users.GetAsync(x => x.Id == request.RequestedUserId);
                if (requestedUser == null)
                {
                    isAuthorized = false;
                }
                else
                {
                    paper = await _unitOfWork.Papers.GetAsync(x => x.Id == request.PaperId,
                    includeProperties: "Exam,Exam.ExamType," +
                                       "Exam.Host,Exam.Reviewer," +
                                       "Exam.ExamType.Levels," +
                                       "Exam.ExamType.ExamTypeConstraints," +
                                       "Exam.ExamType.ExamTypeConstraints.ConstraintDetails," +
                                       "PaperDetails,PaperDetails.Question," +
                                       "PaperDetails.Question.LevelDetail," +
                                       "PaperDetails.Question.LevelDetail.Level," +
                                       "PaperDetails.Question.LevelDetail.Topic," +
                                       "PaperDetails.Question.References," +
                                       "PaperDetails.Question.Answers," +
                                       "PaperDetails.Question.QuestionType"
                                    );
                    if (paper == null) throw new VersionNotFoundException(AppMessages.TARGET_ITEM_NOTFOUND);
                    var exam = paper.Exam;
                    bool examHidden = exam.IsDeleted; // this means the exam is still visible but the papers is not
                    bool examSuspended = exam.IsSuspended; // this means the exam is canceled or suspended so that it actually did not/will not take place
                    bool examInEdit = exam.CreatedDate.AddDays(3) > currentTime; // 1 day for team lead to edit exam info & 2 day to edit & review exam paper
                    bool examInPrepare = currentTime > exam.TakePlaceTime.AddDays(-1) && currentTime < exam.TakePlaceTime; // 24 hours before exam taking place
                    bool examTookPlace = currentTime > exam.TakePlaceTime;
                    bool isTeamLead = requestedUser.RoleId.ToString() == LogicString.Role.TeamLeadRoleId;
                    bool review = exam.ReviewerId == request.RequestedUserId;
                    bool host = exam.HostId == request.RequestedUserId;

                    if (examTookPlace || examSuspended)
                    {
                        isAuthorized = examHidden;
                        // if exam took place or is suspended, everyone can view its papers unless they was locked/hidden
                    }
                    else
                    {
                        // If the exam hasn't taken place & is not suspended
                        if (examInEdit || examInPrepare)
                        {
                            isAuthorized = review || host || isTeamLead;
                            // within the editing time or the exam preparing time (within 2 days after papers were generating),
                            // only host, reviewer and team lead can view the exam
                            // paper were auto-generated 1 days after exam was created
                        }
                        else
                        {
                            isAuthorized = false;
                        }
                    }
                }
                if (!isAuthorized) throw new UnauthorizedAccessException("You are not allowed to view the paper details at this time");
                else
                {
                    await _unitOfWork.AccessLogs.AddAsync(new()
                    {
                        AccessTime = currentTime,
                        ActionName = "Xem đề thi",
                        IpAddress = request.IpAddress,
                        IsSuccess = true,
                        TargetObject = $"{paper.PaperName}",
                        UserId = request.RequestedUserId,
                        Details = $"{requestedUser.UserName} đã yêu cầu xem đề thi {paper.PaperName}"
                    });
                    var mappedResult = _mapper.Map<PaperDetailDisplayDTO>(paper);
                    var source = paper.Exam.ExamType.ExamTypeConstraints.First(x => x.Skill == paper.Skill);
                    mappedResult.PaperConstraint = _mapper.Map<ConstraintToBeDisplayedDTO>(source);
                    response.Result.Add(mappedResult);
                    response.IsSuccess = true;
                }
            }
            catch (VersionNotFoundException e)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.Message = e.Message;
            }
            catch (UnauthorizedAccessException e)
            {
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.Message = e.Message;
            }
            catch (Exception e)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;
        }

        public async Task<APIResponse<PaperGeneralDisplayDTO>> ViewExamPapers(ViewExamPapersListRequest request)

        {
            APIResponse<PaperGeneralDisplayDTO> response = new();
            var exam = await _unitOfWork.Exams.GetAsync(filter: x => x.Id == request.ExamId,
                includeProperties: "ExamType, ExamType.Levels");
            if (exam == null && request.ExamId != null)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Message = "Kỳ thi không tồn tại ~";
                return response;
            }
            Expression<Func<Paper, bool>> filter = x => true;
            if (request.ExamId.HasValue)
            {
                Expression<Func<Paper, bool>> tmpFilter = x => x.ExamId == request.ExamId;
                filter = ExpressionExtension.CombineFilters(filter, tmpFilter);
            }
            if (request.LevelId.HasValue)
            {
                Expression<Func<Paper, bool>> tmpFilter = x => x.Exam.ExamType.LevelId == request.LevelId;
                filter = ExpressionExtension.CombineFilters(filter, tmpFilter);
            }
            if (!string.IsNullOrEmpty(request.NameValueToBeSearched))
            {
                Expression<Func<Paper, bool>> tmpFilter = x => x.PaperName.Contains(request.NameValueToBeSearched);
                filter = ExpressionExtension.CombineFilters(filter, tmpFilter);
            }
            if (request.LowerTakePlaceTimeBound != null)
            {
                Expression<Func<Paper, bool>> tmpFilter = x => x.Exam.TakePlaceTime > request.LowerTakePlaceTimeBound;
                filter = ExpressionExtension.CombineFilters(filter, tmpFilter);
            }
            try
            {
                var papers = await _unitOfWork.Papers.GetAllAsync(filter: filter,
                        includeProperties: "Exam,Exam.ExamType,Exam.ExamType.Levels",
                        orderBy: x => x.OrderByDescending(x => x.CreatedDate),
                        pageNumber: request.PaginationRequest.Page,
                        pageSize: request.PaginationRequest.Size);
                var count = papers.Count;
                if (count == 0)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NoContent;
                    response.Message = "Không có dữ liệu";
                }
                else
                {
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.IsSuccess = true;
                    response.Message = $"{count} ~";
                    response.Result = _mapper.Map<List<PaperGeneralDisplayDTO>>(papers);
                }
            }
            catch (Exception e)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<APIResponse<PaperDetailDisplayDTO>> AutoGenPapersAndNotiToRelevances(Guid examId)
        {
            throw new NotImplementedException();
        }

    }
}
