using AutoMapper;
using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ExamTypeConstraintDTO;
using KEB.Application.DTOs.ExamTypeDTO;
using KEB.Application.Services.Interfaces;
using KEB.Domain.Entities;
using KEB.Domain.Enums;
using KEB.Domain.ValueObject;
using KEB.Infrastructure.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Implementations
{
    public class ExamTypeService : IExamTypeService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ExamTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<APIResponse<ExamTypeComplexDisplayDTO>> AddExamTypeAsync(AddExamTypeRequest request)

        {
            var response = new APIResponse<ExamTypeComplexDisplayDTO>
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest
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
                var level = await _unitOfWork.Levels.GetLevelById(request.LevelId);
                if (level == null)
                {
                    response.Message = AppMessages.EXAM_TYPE_LEVEL_NOT_EXIST;
                    return response;
                }
                if (string.IsNullOrEmpty(request.ExamTypeName))
                {
                    response.Message = AppMessages.EXAM_TYPE_EMPTY_NAME;
                    return response;
                }
                
                    var normalizedName = request.ExamTypeName.Trim().ToLower();
                    var existedType = await _unitOfWork.ExamTypes.GetAsync(x => x.ExamTypeName.ToLower() == normalizedName);
                    if (existedType != null)
                    {
                        response.Message = AppMessages.EXAM_TYPE_EXISTS;
                        return response;
                    }
                
                if (request.ExamTypeConstraints == null || request.ExamTypeConstraints.Count == 0)
                {
                    response.Message = AppMessages.EXAM_TYPE_HAS_NO_STRUCTURE;
                    return response;
                }

                await _unitOfWork.BeginTransactionAsync();
                ExamType newExamType = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedDate = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    CreatedBy = requestedUser.Id,
                    UpdatedBy = requestedUser.Id,
                    ExamTypeName = request.ExamTypeName.Trim(),
                    IsDeleted = false,
                    LevelId = level.Id,
                    Exams = []
                };
                await _unitOfWork.ExamTypes.AddAsync(newExamType);
                foreach (var constraint in request.ExamTypeConstraints)
                {

                    constraint.ExamTypeId = newExamType.Id;
                    await AddExamTypeConstraint(constraint, constraint.Skill, newExamType.LevelId);
                }
                ExamTypeComplexDisplayDTO mappedExamType = _mapper.Map<ExamTypeComplexDisplayDTO>(newExamType);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                response.Result.Add(mappedExamType);
                response.StatusCode = HttpStatusCode.Created;
                response.Message = AppMessages.EXAM_TYPE_CREATE_SUCCESS;
                response.IsSuccess = true;
            }
            catch (ArgumentException ex)
            {
                await _unitOfWork.RollbackAsync();
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = ex.Message;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = "Lỗi" + ex;
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return response;
        }
        private async Task AddExamTypeConstraint(InputExamTypeConstraintDTO constraint, Skill skill, Guid levelId)
        {
            await ValidateExamTypeConstraint(constraint, skill);
            ExamTypeConstraint newConstraint = new()
            {
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                DurationInMinutes = constraint.DurationInMinutes,
                ExamTypeId = constraint.ExamTypeId ?? throw new Exception("Constraint phải link đến một Examtype~"),
                NumberOfPaper = constraint.NumberOfPapers,
                Skill = skill,
                TotalNumberOfQuestions = constraint.TotalNumberOfQuestions,
                ConstraintDetails = new List<ConstraintDetail>()
            };
            await _unitOfWork.ExamTypesConstraints.AddAsync(newConstraint);
            Console.WriteLine("loi o day" + constraint.ConstraintDetails);
            await ValidateExamTypeConstraintDetails(constraint.ConstraintDetails, skill, levelId);
            foreach (var detail in constraint.ConstraintDetails)
            {
                ConstraintDetail constraintDetail = new()
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Difficulty = detail.Difficulty,
                    ExamTypeConstraintId = newConstraint.Id,
                    IsMultipleChoice = detail.IsMultipleChoice,
                    MarkPerQuestion = detail.MarkPerQuestion,
                    NumberOfQuestion = detail.NumberOfQuestions,
                    QuestionTypeId = detail.QuestionTypeId,
                    TopicId = detail.TopicId,
                };
                await _unitOfWork.ConstraintDetails.AddAsync(constraintDetail);
                //await ValidateExamTypeConstraintDetail(constraintDetail, skill);
               // newConstraint.ConstraintDetails.Add(constraintDetail);
            }
            await _unitOfWork.SaveChangesAsync();
        }
        private async Task ValidateExamTypeConstraint(InputExamTypeConstraintDTO constraint, Skill skill)
        {
            var validationMessages = new List<string> { $"Bài thi {skill}:" };
            Console.WriteLine($"Validating exam type constraint for skill: {skill}");
            Console.WriteLine($"Number of papers: {constraint.NumberOfPapers}");
            Console.WriteLine($"Total number of questions: {constraint.TotalNumberOfQuestions}");
            Console.WriteLine($"Duration in minutes: {constraint.DurationInMinutes}");

            // 1. Số đề thi phải >= 1
            if (constraint.NumberOfPapers < 1)
            {
                validationMessages.Add(AppMessages.EXAM_TYPE_NUMOFPAPERS_LESSTHAN1);
            }
            var details = constraint.ConstraintDetails?.ToList();
            if (details == null || details.Count == 0)
            {
                validationMessages.Add("Danh sách constraint detail đang trống. Vui lòng thêm ít nhất 1 chủ đề.");
            }
            // 2. Tổng số câu hỏi trong từng detail phải đúng với tổng số constraint
            int sumOfDetailQuestions = details?.Sum(x => x.NumberOfQuestions) ?? 0;
            if (sumOfDetailQuestions != constraint.TotalNumberOfQuestions)
            {
                throw new ArgumentException($"Bài thi {skill}:<br/>Tổng số lượng câu hỏi ({sumOfDetailQuestions}) của các detail không bằng số lượng câu hỏi constraint ({constraint.TotalNumberOfQuestions})");

                validationMessages.Add("Tổng số lượng câu hỏi của từng detail không bằng số lượng câu hỏi của constraint");
            }

            // 3. Kiểm tra ngân hàng câu hỏi có đủ không
            var availableQuestions = await _unitOfWork.Questions.GetAllAsync(x => x.QuestionType.Skill == skill);
            int availableCount = availableQuestions.Count;

            if (availableCount < constraint.TotalNumberOfQuestions)
            {
                int missing = constraint.TotalNumberOfQuestions - availableCount;
                validationMessages.Add($"Ngân hàng câu hỏi không có đủ {constraint.TotalNumberOfQuestions} câu hỏi {skill}. Vui lòng thêm {missing} câu hỏi trước nhé ~");
            }

            // 4. Số câu hỏi tối thiểu
            if (constraint.TotalNumberOfQuestions < SystemDataFormat.MINIMUM_NUMOFQUESTIONS_FOR_EACHPAPER)
            {
                validationMessages.Add($"Số câu hỏi của một bài thi ít nhất phải là {SystemDataFormat.MINIMUM_NUMOFQUESTIONS_FOR_EACHPAPER} câu");
            }

            // 5. Số câu hỏi tối đa
            if (constraint.TotalNumberOfQuestions > SystemDataFormat.MAXIMUM_NUMOFQUESTIONS_FOR_EACHPAPER)
            {
                validationMessages.Add($"Số câu hỏi của một bài thi nhiều nhất chỉ là {SystemDataFormat.MAXIMUM_NUMOFQUESTIONS_FOR_EACHPAPER} câu");
            }

            // 6. Nếu là bài nghe, kiểm tra thời lượng tối thiểu
            //if (skill == Skill.Nghe)
            //{
            //    int requiredSeconds = _unitOfWork.Questions.MinimumTotalDurationOfNListeningQuestion(constraint.TotalNumberOfQuestions);
            //    int availableSeconds = constraint.DurationInMinutes * 60;

            //    if (requiredSeconds > availableSeconds)
            //    {
            //        int requiredMinutes = requiredSeconds / 60;
            //        int remainingSeconds = requiredSeconds % 60;

            //        validationMessages.Add($"Bài thi gồm {constraint.TotalNumberOfQuestions} câu hỏi nghe sẽ cần thời lượng nhỏ nhất là {requiredMinutes} phút và {remainingSeconds} giây");
            //    }
            //}

            // 7. Nếu có lỗi => throw exception
            if (validationMessages.Count > 1)
            {
                throw new ArgumentException(string.Join("<br/>", validationMessages));
            }
        }
        private async Task ValidateExamTypeConstraintDetails(IEnumerable<InputExamTypeConstraintDetailDTO> details, Skill skill, Guid levelId, int numOfPaper = 1)
        {
            
            var groupCount = details.GroupBy(x => new { x.Difficulty, x.QuestionTypeId, x.TopicId, x.IsMultipleChoice }).Count();
            if (groupCount < details.Count())
                throw new ArgumentException(AppMessages.EXAM_TYPE_DUPLICATE_CONSTRAINT_DETAIL);

            List<(bool IsOk, string Message)> detailsValidateResult = [];
            int detailIndex = 1;
            foreach (var detail in details)
            {
                List<string> singleDetailValidateResult = [];
                var topic = await _unitOfWork.Topics.GetByIdAsync(detail.TopicId);
                if (topic == null)
                {
                    singleDetailValidateResult.Add($"Không tìm thấy chủ đề");
                }
                var questiontype = await _unitOfWork.QuestionTypes.GetByIdAsync(detail.QuestionTypeId);
                if (questiontype == null)
                {
                    singleDetailValidateResult.Add($"Không tìm thấy loại câu hỏi");
                }
                else
                {
                    if (questiontype.Skill != skill)
                        singleDetailValidateResult.Add($"Kỹ năng {skill} không match với loại câu hỏi");
                }
                if (detail.NumberOfQuestions > SystemDataFormat.MAXIMUM_NUMOFQUESTIONS_FOR_EACHDETAIL_OF_PAPER)
                {
                    singleDetailValidateResult.Add($"Mỗi đề thi {skill} không được có quá " +
                        $"{SystemDataFormat.MAXIMUM_NUMOFQUESTIONS_FOR_EACHDETAIL_OF_PAPER} " +
                        $"câu hỏi cho một dạng câu hỏi và chủ đề");
                }
                var satisfiedQuestions = await _unitOfWork.Questions.GetAllAsync(filter:
                            x => (x.QuestionType.Skill == skill)
                            && x.Status == QuestionStatus.Ok
                            && x.QuestionTypeId == detail.QuestionTypeId
                            && (x.Difficulty == detail.Difficulty)
                            && x.IsMultipleChoice == detail.IsMultipleChoice
                            && x.LevelDetail.LevelId == levelId
                            && x.LevelDetail.TopicId == detail.TopicId
                            );
                if (satisfiedQuestions == null || satisfiedQuestions.Count < detail.NumberOfQuestions)
                {
                    singleDetailValidateResult.Add($"Ngân hàng câu hỏi không đủ {detail.NumberOfQuestions} câu hỏi khác nhau "
                        + $"thuộc loại {skill.ToString().ToLower()}, "
                        + $"dạng {(detail.IsMultipleChoice ? "trắc nghiệm, " : "tự luận, ")}"
                        + $"độ khó {detail.Difficulty} và "
                        + $"thuộc chủ đề {topic?.TopicName ?? "Unknown"} cho {numOfPaper} đề thi mới");
                }
                detailsValidateResult.Add((singleDetailValidateResult.Count == 0, string.Join("~", singleDetailValidateResult)));
                detailIndex++;
            }
            var detailErrorList = detailsValidateResult.Where(x => !x.IsOk).Select(x => x.Message).ToList();
            if (detailErrorList.Count != 0)
            {
                throw new ArgumentException(string.Join("<br/>", detailErrorList));
            }
        }
        public async Task<APIResponse<ExamTypeComplexDisplayDTO>> DeleteExamTypeAsync(Delete request)

        {
            APIResponse<ExamTypeComplexDisplayDTO> response = new() { IsSuccess = false };
            try
            {
                var requestedUser = await _unitOfWork.Users.GetByIdAsync(request.RequestedUserId);
                if (requestedUser == null || requestedUser.RoleId.ToString() != LogicString.Role.TeamLeadRoleId)
                {
                    response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                    response.Message = AppMessages.NO_PERMISSION;
                    return response;
                }
                var target = await _unitOfWork.ExamTypes.GetAsync(x => x.Id == request.TargetObjectId,
                                    includeProperties: "Level,Exams,ExamTypeConstraints," +
                                                       "ExamTypeConstraints.ConstraintDetails," +
                                                       "ExamTypeConstraints.ConstraintDetails.QuestionType," +
                                                       "ExamTypeConstraints.ConstraintDetails.Topic");
                if (target == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                    return response;
                }
                var examCount = target.Exams.Count();
                if (examCount > 0)
                {
                    response.StatusCode = System.Net.HttpStatusCode.Conflict;
                    response.Message = AppMessages.EXAM_TYPE_DELETE_FAIL_CUZ_CONFLICT;
                    return response;
                }

                await _unitOfWork.ExamTypes.DeleteExamType(target, true);
                response.IsSuccess = true;
                response.Message = AppMessages.EXAM_TYPE_DELETE_SUCCESS;
                response.Result.Add(_mapper.Map<ExamTypeComplexDisplayDTO>(target));
            }
            catch (Exception)
            {
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.Message = AppMessages.INTERNAL_SERVER_ERROR;
            }
            return response;
        }

        public async Task<APIResponse<object>> EditExamTypeAsync(FullEditExamTypeRequest request)

        {
            APIResponse<object> response = new()
            {
                IsSuccess = false,
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            List<string> examtypeChanged = [];
            var requestedUser = await _unitOfWork.Users.GetByIdAsync(request.RequestedUserId);
            if (requestedUser == null || requestedUser.RoleId.ToString() != LogicString.Role.TeamLeadRoleId)
            {
                response.StatusCode = System.Net.HttpStatusCode.Forbidden;
                response.Message = LogicString.Permission.NoPermission;
                return response;
            }
            var target = await _unitOfWork.ExamTypes.GetAsync(x => x.Id == request.TargetObjectId,
                                includeProperties: "Levels,Exams,ExamTypeConstraints," +
                                                   "ExamTypeConstraints.ConstraintDetails," +
                                                   "ExamTypeConstraints.ConstraintDetails.QuestionType," +
                                                   "ExamTypeConstraints.ConstraintDetails.Topic");
            if (target == null)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                return response;
            }
            var examCount = target.Exams.Count();
            if (examCount > 0)
            {
                //response.Message = $"Đã có {examCount} kỳ thi của loại kỳ thi này ~ hong cho sửa";
                response.Message = AppMessages.EXAM_TYPE_HAS_BEEN_APPLIED;
                return response;
            }
            Level oldLevel = target.Levels;
            Level? newLevel = target.Levels;
            if (request.NewLevelId != target.LevelId)
            {
                newLevel = await _unitOfWork.Levels.GetByIdAsync(request.NewLevelId);
                if (newLevel == null)
                {
                    response.Message = "Không tìm thấy trình độ";
                    return response;
                }
                else
                {
                    target.LevelId = newLevel.Id;
                    examtypeChanged.Add("Đổi trình độ");
                }
            }
            if (request.NewExamTypeName.Trim().ToLower() != target.ExamTypeName.Trim().ToLower())
            {
                if (string.IsNullOrEmpty(request.NewExamTypeName))
                {
                    response.Message = AppMessages.EXAM_TYPE_EMPTY_NAME;
                    return response;
                }
                else
                {
                    target.ExamTypeName = request.NewExamTypeName;
                    examtypeChanged.Add("Đổi tên");
                }
            }

            List<(bool IsOk, string Message)> detailsValidateResult = [];
            List<ConstraintDetail> constraintDetailsList = [];
            if (request.ConstraintsChanged)
            {
                if (request.Constraints == null || request.Constraints.Count == 0)
                {
                    response.Message = AppMessages.EXAM_TYPE_HAS_NO_STRUCTURE;
                    return response;
                }
                examtypeChanged.Add("Sửa constraints");
            }
            try
            {
                if (examtypeChanged.Count > 0)
                {
                    await _unitOfWork.BeginTransactionAsync();
                    await _unitOfWork.ExamTypes.DeleteExamType(target, deleteConstraintOnly: true);
                    //foreach (var constraint in request.Constraints)
                    //{
                    //    await AddExamTypeConstraint(constraint, constraint.Skill, target.LevelId);
                    //}
                    await _unitOfWork.CommitAsync();
                    response.Result.Add(_mapper.Map<ExamTypeComplexDisplayDTO>(target));
                    //response.Message = string.Join("<br/>", examtypeChanged);
                    response.Message = AppMessages.EXAM_TYPE_UPDATE_SUCCESS;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    response.Result.Add(_mapper.Map<ExamTypeComplexDisplayDTO>(target));
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.IsSuccess = true;
                    response.Message = AppMessages.NO_CHANGES_DETECTED;
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

        public async Task<APIResponse<ExamTypeComplexDisplayDTO>> GetExamTypeDetails(Guid examTypeId)

        {
            APIResponse<ExamTypeComplexDisplayDTO> response = new();

            try
            {
                var examtype = await _unitOfWork.ExamTypes.GetAsync(x => x.Id == examTypeId,
                                    includeProperties: "Levels,Exams,ExamTypeConstraints," +
                                                       "ExamTypeConstraints.ConstraintDetails," +
                                                       "ExamTypeConstraints.ConstraintDetails.QuestionType," +
                                                       "ExamTypeConstraints.ConstraintDetails.Topic,");
                if (examtype == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.NoContent;
                    response.Message = AppMessages.TARGET_ITEM_NOTFOUND;
                }
                else
                {
                    bool hasListenPaper = examtype.ExamTypeConstraints.Any(x => x.Skill == Skill.Nghe);
                    bool hasSpeakPaper = examtype.ExamTypeConstraints.Any(x => x.Skill == Skill.Nói);
                    bool hasReadPaper = examtype.ExamTypeConstraints.Any(x => x.Skill == Skill.Đọc);
                    bool hasWritePaper = examtype.ExamTypeConstraints.Any(x => x.Skill == Skill.Viết);
                    var mappedResult = _mapper.Map<ExamTypeComplexDisplayDTO>(examtype);
                    if (!hasListenPaper) mappedResult.PaperConstraints.Add(new ConstraintToBeDisplayedDTO { Skill = Skill.Nghe });
                    if (!hasSpeakPaper) mappedResult.PaperConstraints.Add(new ConstraintToBeDisplayedDTO { Skill = Skill.Nói });
                    if (!hasReadPaper) mappedResult.PaperConstraints.Add(new ConstraintToBeDisplayedDTO { Skill = Skill.Đọc });
                    if (!hasWritePaper) mappedResult.PaperConstraints.Add(new ConstraintToBeDisplayedDTO { Skill = Skill.Viết });
                    response.Result.Add(mappedResult);
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
        public async Task<APIResponse<ExamTypeGeneralDisplayDTO>> GetExamTypesAsync()

        {
            APIResponse<ExamTypeGeneralDisplayDTO> response = new();
            try
            {
                var queriedResult = await _unitOfWork.ExamTypes.GetAllAsync(includeProperties: "Levels,ExamTypeConstraints,Exams");
                var count = queriedResult.Count;
                if (count == 0)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NoContent;
                    response.Message = AppMessages.NO_CONTENT;
                }
                else
                {
                    response.Result = _mapper.Map<List<ExamTypeGeneralDisplayDTO>>(queriedResult);
                    response.Message = count + "";
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
    }
}
