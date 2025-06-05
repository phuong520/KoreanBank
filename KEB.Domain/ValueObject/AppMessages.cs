using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.ValueObject
{
    public class AppMessages
    {
        #region Common 
        public const string INTERNAL_SERVER_ERROR = "Có lỗi phía server"; // Có lỗi ở phía server
        public const string TARGET_ITEM_NOTFOUND = "Không tìm thấy dữ liệu"; // Không tìm thấy dữ liệu, chắc là bị xóa mất ruii ~
        public const string NO_CHANGES_DETECTED = "Không có thay đổi gì"; // Không có thay đổi gì ~
        public const string NO_PERMISSION = "Bạn không có quyền thực hiện tác vụ này"; // Bạn 0 đủ trình để thực hiện chức năng này ~ liên hệ Admin nhen
        public const string NO_CONTENT = "Không có dữ liệu"; // 0 có dữ liệu
        #endregion Common 
        #region Login
        public const string USERNAME_REQUIRED = "MS-01"; // Username field is empty
        public const string PASSWORD_REQUIRED = "MS-02"; // Password field is empty
        public const string WRONG_LOGIN_INFO = "Sai tên đăng nhập hoặc mật khẩu"; // Username or password is not correct
        public const string LOCKED_ACCOUNT = "Tài khoản bị khóa, vui lòng liên hệ Admin hỗ trợ"; // Account is locked when login
        public const string SESSION_EXPIRED = "MS-05"; // Login session has expired after 60 minutes

        public const string LOGIN_SUCCESS = "Đăng nhập thành công"; // Login Successfully
        public const string LOGIN_SUCCESS_BUT_PASS_NEED_CHANGED = "MS-118"; // Login Successfully
        #endregion Login
        #region Reset & Change pass
        public const string RESET_PASSWORD_SUCCESS = "MS-06"; // Reset password successfully
        public const string EMAIL_REQUIRED = "MS-07"; // Email field is empty
        public const string INVALID_EMAIL = "MS-08"; // Email is invalid
        public const string NEWPASSWORD_NOT_CHANGE = "MS-09"; // New password matches with current password
        public const string PASSWORD_WRONG_FORMAT = "MS-10"; // Password has wrong format
        public const string CONFIRM_PASSWORD_NOT_MATCH = "MS-11"; // Confirm password does not match current password
        public const string EMPTY_CURRENT_PASSWORD = "MS-12"; // Current password is empty
        public const string EMPTY_NEW_PASSWORD = "MS-13"; // New password field is empty
        public const string EMPTY_CONFIRM_PASSWORD = "MS-14"; // Confirm password field is empty

        public const string CHANGE_PASS_SUCCESS = "MS-119"; // Change password success
        public const string WRONG_OLD_PASSWORD = "MS-120"; // Wrong old password
        public const string EMAIL_EXISTED = "MS-122"; // EMAIL has been registered for an other account
        public const string ACCOUNT_NOT_FOUND = "MS-160"; // Account does not be found

        #endregion Reset & Change pass
        #region Create account & changestatus
        public const string EMPTY_FULL_NAME = "MS-15"; // Full name field is empty
        public const string AGE_UNDER_18 = "MS-16"; // Age is smaller than 18
        public const string UPDATE_PROFILE_SUCCESS = "Cập nhật vai trò thành công"; // Update profile successfully
        public const string UPDATE_AVATAR_SUCCESS = "MS-18"; // Update avatar successfully
        public const string EMPTY_DOB = "MS-19"; // Date of birth field is empty
        public const string EMPTY_ROLE = "MS-20"; // Role field is empty
        public const string EMPTY_SEX = "MS-21"; // Sex field is empty
        public const string CREATE_ACCOUNT_SUCCESS = "Tạo tài khoản thành công"; // Create account successfully
        public const string INVALID_FULL_NAME = "MS-23"; // Full name is invalid
        public const string ACTIVATE_ACCOUNT_SUCCESS = "Mở khóa tài khoản thành công"; // Activate account successfully
        public const string ACCOUNT_ACTIVATION_CONFIRM = "MS-26"; // Confirm account activation
        public const string LOCK_ACCOUNT_SUCCESS = "Tài khoản đã bị khóa"; // Lock account successfully
        public const string ACCOUNT_LOCK_CONFIRM = "MS-28"; // Confirm account lock
        #endregion Create account & changestatus

        #region References
        public const string EMPTY_REFERENCE = "MS-30"; // Reference field is empty
        public const string EMPTY_PUBLISHED_YEAR = "MS-31"; // Published year field is empty
        public const string EMPTY_REFERENCE_LINK = "MS-32"; // Reference link field is empty
        public const string EMPTY_AUTHOR = "MS-33"; // Author field is empty
        public const string REFERENCE_CREATE_SUCCESS = "Tạo tài liệu tham khảo thành công"; // Create reference successfully
        public const string REFERENCE_UPDATE_SUCCESS = "Cập nhật tài liệu tham khảo thành công"; // Update reference successfully
        public const string REFERENCE_DELETE_SUCCESS = "Xóa tài liệu tham khảo thành công"; // Delete reference successfully
        public const string REFERENCE_DELETE_CONFIRM = "MS-37"; // Confirm reference deletion

        public const string REFERENCE_EXISTED = "Tài liệu tham khảo này đã tồn tại"; // Reference exists in the system
        public const string REFERENCE_DELETE_FAILED = "Lỗi khi xóa tài liệu tham khảo vì có dữ liệu liên quan"; // Delete reference failed
        public const string INVALID_REFERENCE_YEAR = "MS-160"; // reference's published year > current year
        #endregion References
        #region Questiontypes
        public const string QUESTION_TYPE_EMPTY_NAME = "MS-38"; // Question type is empty
        public const string QUESTION_TYPE_EMPTY_CONTENT = "MS-39"; // Content field is empty
        public const string QUESTION_TYPE_CREATE_SUCCESS = "Tạo loại câu hỏi thành công"; // Create question type successfully
        public const string QUESTION_TYPE_UPDATE_SUCCESS = "Cập nhật loại câu hỏi thành công"; // Update question type successfully
        public const string QUESTION_TYPE_DELETE_CONFIRM = "MS-42"; // Confirm question type deletion
        public const string QUESTION_TYPE_DELETE_SUCCESS = "Xóa loại câu hỏi thành công"; // Delete question type successfully

        public const string QUESTION_TYPE_NAME_EXISTED = "MS-70"; // Question type name exists in the system
        public const string QUESTION_TYPE_DELETE_FAILED = "Lỗi khi xóa vì có dữ liệu liên quan"; // Delete question type failed
        #endregion Questiontypes
        #region Levels & Topics
        public const string LEVEL_CREATE_SUCCESS = "Tạo cấp độ thành công"; // Create level successfully
        public const string LEVEL_EMPTY_NAME = "MS-45"; // Level name field is empty
        public const string LEVEL_UPDATE_SUCCESS = "Cập nhật cấp độ thành công"; // Update level successfully
        public const string LEVEL_DELETE_SUCCESS = "Xóa cấp độ thành công"; // Delete level successfully
        public const string LEVEL_DELETE_CONFIRM = "MS-48"; // Confirm level deletion
        public const string LEVEL_ADDTOPIC_SUCCESS = "MS-49"; // Add topic for level successfully
        public const string LEVEL_NOTOPIC_SELECTED = "MS-50"; // Topic field is empty

        public const string TOPIC_REMOVE_FROM_LEVEL_SUCCESS = "MS-51"; // Remove topic out of level successfully
        public const string TOPIC_REMOVE_FROM_LEVEL_FAILED = "MS-52"; // Remove topic out of level failed
        public const string TOPIC_CREATE_SUCCESS = "Tạo chủ đề thành công"; // Create topic successfully
        public const string TOPIC_EMPTY_NAME = "MS-54"; // Topic name field is empty
        public const string TOPIC_NOLEVEL_SELECTED = "MS-55"; // Topic must be create with a level
        public const string TOPIC_UPDATE_SUCCESS = "Cập nhật chủ đề thành công"; // Update topic successfully
        public const string TOPIC_DELETE_SUCCESS = "Xóa chủ đề thành công"; // Delete topic successfully
        public const string TOPIC_DELETE_CONFIRM = "MS-58"; // Confirm topic deletion

        public const string LEVEL_EXISTED = "MS-71"; // Level exists in the system
        public const string TOPIC_EXISTED = "MS-72"; // Topic exists in the system

        public const string LEVEL_DELETE_FAILED = "Không thể xóa cấp độ vì có dữ liệu liên quan"; // Delete level failed
        public const string TOPIC_DELETE_FAILED = "Không thể xóa chủ đề này vì có dữ liệu liên quan"; // Delete topic failed

        public const string TOPIC_ASSIGN_EMPTY = "MS-145"; // Assign no topics for level


        #endregion Levels & Topics

        #region Questions
        public const string QUESTIONS_ARE_PROCESSING = "MS-59"; // The system processes checking questions
        public const string QUESTION_EMPTY_DIFFICULTY = "MS-60"; // Difficulty is empty
        public const string QUESTION_EMPTY_TASK = "MS-61"; // Task field is empty
        public const string QUESTION_EMPTY_QUESTION_CONTENT = "MS-62"; // Question content is empty
        public const string QUESTION_ANSWER_REQUIRED = "MS-63"; // Answer field is empty
        public const string QUESTION_CORRECT_ANSWER_REQUIRED = "MS-64"; // Correct answer field is empty

        public const string QUESTION_AUDIO_REQUIRED = "MS-65"; // Upload 0 audio files
        public const string DUPLICATE_CHECKING = "MS-66"; // System process checking duplicated
        public const string QUESTION_WORDFILE_REQUIRED = "MS-67"; // Upload 0 file Word
        public const string QUESTION_EXCELFILE_REQUIRED = "MS-68"; // Upload 0 file Excel
        public const string QUESTION_UPDATE_SUCCESS = "MS-73"; // Update question successfully
        public const string QUESTION_CONFIRM_DELETE = "MS-74"; // Confirm question deletion
        public const string QUESTION_DELETE_SUCCESS = "MS-75"; // Delete question successfully

        public const string UPLOAD_QUESTION_FAILED = "MS-140";

        public const string UPLOAD_QUESTION_FROM_EXCEL_SUCCESS = "MS-139";

        public const string CANNOT_ACCESS_TASK_OF_OTHER_USER = "MS-141";
        public const string TASK_NOTFOUND = "MS-143"; // Muon them cau hoi cho 1 nhiem vu nma d tim thay nhiem vu do ~
        public const string ONLY_PENDING_QUESTION_CAN_BE_EDITED = "MS-144"; // Only pending question can be edited by team lead
        public const string CANNOT_EDIT_QUESTION_OF_OTHER = "MS-145";
        public const string REASON_REQUIRED_WHEN_DENYING_QUESTION = "MS-146";
        public const string UNSUPPORTED_MEDIA_FILE = "MS-147";
        public const string AUDIO_DURATION_EXCEED_LIMIT = "MS-148";
        #endregion Questions

        #region Import question tasks
        public const string TASK_EMPTY_QUESTION_TYPE_FIELD = "MS-76"; // Question type field is empty
        public const string TASK_ASSIGNED_SUCCESS = "MS-77"; // Create task successfully
        public const string EMPTY_SKILL = "MS-78"; // Skill field is empty
        public const string QUESTIONS_BELOW_MINIMUM = "MS-79"; // Number of questions is smaller than 1
        public const string TASK_UPDATE_SUCCESS = "MS-80"; // Update task successfully
        public const string DEADLINE_TOO_HURRY = "MS-81"; // Deadline is too hurry to do
        public const string TASK_DELETE_SUCCESS = "MS-82"; // Delete task successfully
        public const string TASK_DELETE_CONFIRM = "MS-83"; // Task deletion confirmation

        public const string EMPTY_TASK_LIST = "MS-129"; // Empty task list
        public const string ASSIGNEE_NOT_EXISTED = "MS-130"; // Assignee of task did not exist in the system
        public const string ASSIGNEE_LOCKED = "MS-131"; // Assignee's account has been deactivated
        public const string TASK_IS_IN_PROCESS = "MS-132"; // Assignee's started the task
        #endregion Import question tasks

        #region Exam type
        public const string EXAM_TYPE_LEVEL_NOT_EXIST = "MS-133"; // The level that was assigned to exam type did not exist in the systems
        public const string EXAM_TYPE_DELETE_FAIL_CUZ_CONFLICT = "MS-134"; //
        public const string EXAM_TYPE_NUMOFPAPERS_LESSTHAN1 = "MS-135"; //
        public const string EXAM_TYPE_DUPLICATE_CONSTRAINT_DETAIL = "MS-136"; // Constraint details không được trùng lặp. Một trong 4 thuộc tính: chủ đề, độ khó, dạng câu hỏi (TN-TL), loại câu hỏi phải khác nhau ~
        public const string EXAM_TYPE_HAS_BEEN_APPLIED = "MS-137"; // Có kỳ thi của loại kỳ thi này r nên 0 cho sửa nữa ~

        public const string EXAM_TYPE_EXISTS = "MS-84"; // Exam type name exists in the system
        public const string EXAM_TYPE_EMPTY_NAME = "MS-85"; // Exam type field is empty
        public const string EXAM_TYPE_HAS_NO_STRUCTURE = "MS-86"; // Exam type does not have any exam paper’s structure / Loại kỳ thi phải bao gồm ít nhất một bài thi ~
        public const string TOTAL_MARK_BELOW_MINIMUM = "MS-87"; // Total mark is smaller than 100
        public const string LISTENING_CONSTRAINT_NOT_MET = "MS-88"; // Questions in question bank do not meet listening exam paper’s constraints
        public const string SPEAKING_CONSTRAINT_NOT_MET = "MS-89"; // Questions in question bank do not meet speaking exam paper’s constraints
        public const string READING_CONSTRAINT_NOT_MET = "MS-90"; // Questions in question bank do not meet reading exam paper’s constraints
        public const string WRITING_CONSTRAINT_NOT_MET = "MS-91"; // Questions in question bank do not meet writing exam paper’s constraints
        public const string DUPLICATE_TEST_CONSTRAINT = "MS-92"; // Test’s constraints are duplicated
        public const string DURATION_BELOW_AUDIO = "MS-93"; // Test duration is smaller than total of audio duration
        public const string QUESTIONS_BELOW_LIMIT = "MS-94"; // Number of questions smaller than 1
        public const string EXAM_PAPER_BELOW_LIMIT = "MS-95"; // Number of exam paper smaller than 1
        public const string MARK_PER_QUESTION_BELOW_MINIMUM = "MS-96"; // Mark per question smaller than 1

        public const string EXAM_TYPE_CREATE_SUCCESS = "MS-98"; // Create exam type successfully
        public const string EXAM_TYPE_UPDATE_SUCCESS = "MS-99"; // Update exam type successfully
        public const string EXAM_TYPE_DELETE_SUCCESS = "MS-100"; // Delete exam type successfully
        public const string EXAM_TYPE_DELETE_CONFIRM = "MS-101"; // Exam type deletion confirmation
        #endregion Exam type

        #region Exam and test (exam paper)
        public const string EXAM_DELETE_FAILED_CUZ_CONFLICT = "MS-138"; // Exam link to several papers hence can not perform delete

        public const string EMPTY_EXAM_NAME = "MS-102"; // Exam name field is empty
        public const string EMPTY_EXAM_DATE = "MS-103"; // Exam date field is empty
        public const string EXAM_DATE_BEFORE_CURRENT = "MS-104"; // Exam date before current date or equal current date
        public const string EMPTY_HOST = "MS-105"; // Host field is empty
        public const string EMPTY_REVIEWER = "MS-106"; // Reviewer field is empty
        public const string EXAM_NAME_EXISTS = "Tên kỳ thi đã có"; // Exam name exists in the system
        public const string EXAM_CREATE_SUCCESS = "Tạo kỳ thi thành công"; // Create exam successfully
        public const string EXAM_UPDATE_SUCCESS = "Cập nhật kỳ thi thành công"; // Update exam successfully
        public const string EXAM_DELETE_SUCCESS = "Xóa kỳ thi thành công"; // Delete exam successfully
        public const string EXAM_DELETE_CONFIRM = "MS-111"; // Exam deletion confirmation
        public const string COMMENT_SUCCESS = "MS-112"; // Comment on test successfully
        public const string TEST_UPDATE_SUCCESS = "MS-113"; // Update test successfully
        public const string CONSTRAINTS_NOT_MET = "MS-114"; // Test does not meet constraints
        public const string EMPTY_COMMENT = "MS-115"; // Comment field is empty
        public const string REVIEW_SEND_SUCCESS = "Đã gửi đến reviewer"; // Send to reviewer successfully

        public const string UPDATE_EXAM_HAS_PAPER = "Không thể cập nhật vì kỳ thi đang có đề thi."; // Can not update exam because it has been containing several exam papers
        public const string CAN_ONLY_UPDATE_EXAM_WITHIN_LIMIT_TIME = "Chỉ có thể sửa trong thời gian quy định"; // Can not update exam because it has been containing several exam papers
        public const string CAN_ONLY_DELETE_EXAM_IN_LIMIT_TIME = "MS-150";
        public const string CAN_NOT_HIDE_OR_DISPLAY_EXAM_BEFORE_HAPPENING = "MS-151";
        #endregion Exam and test (exam paper)


        public const string STATISTIC_BY_MONTH = "MS-150"; // Just view statistics in same year

    }
}
