using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.ValueObject
{
    public class LogicString
    {
        public class ExceptionMessage
        {
            public const string ITEM_NOTEXIST = "Dữ liệu không tồn tại!";
            public const string USER_NOTEXIST = "Người dùng không tồn tại!";
            public const string QTYPE_NOTFOUND = "Dòng {0} không tìm thấy loại câu hỏi!";
            public const string SHEET_NAME_NOTFOUND = "Không tìm thấy trang tính có tên {0}!";
            public const string WRONG_TEMPLATE_FORMAT = "Template không đúng, vui lòng tải và sử dụng template dành cho import câu hỏi của hệ thống!";
        }
        public class AccessLogConstant
        {
            // Actions
            public const string LOGINACTION = "Đăng nhập";
            public const string RESETPASS_ACTION = "Đặt lại mật khẩu";
            public const string GET_ACTION = "Xem dữ liệu";
            public const string UPDATE_ACTION = "Chỉnh sửa {0}";
            public const string DELETE_ACTION = "Xóa {0}";
            public const string CREATE_ACTION = "Tạo mới {0}";
            public const string MULTIPLE_UPLOAD_EXCEL = "Thêm câu hỏi từ file Excel";
            public const string MULTIPLE_UPLOAD_WORD = "Thêm câu hỏi từ file Word";
            public const string UPLOADFILE_ACTION = "Tải tệp lên Azure blob";
            // Controller
            public const string USERS_CONTROLLER = "Users";
            public const string QUESTIONS_CONTROLLER = "Questions";
            public const string QUESTIONTYPES_CONTROLLER = "Question types";
            public const string COMMON_CONTROLLER = "Common";
            public const string ANSWER_CONTROLLER = "Answers";
            public const string LEVEL_CONTROLLER = "Levels";
            public const string IMPORTFILE_TEMPLATE_CONTROLLER = "Import templates";

            // 
            // Success details message
            public const string ADDQUESTIONS_FROMEXCEL_SUCCESS = "{0} tải lên thành công {1} câu hỏi từ Excel";
            public const string DEACTIVE_SUCCESS = "{0} đã khóa tài khoản của {1}";
            public const string ACTIVE_SUCCESS = "{0} đã mở khóa tài khoản của {1}";

            public const string LOGIN_SUCCESS = "{0} đăng nhập thành công";
            public const string RESETPASS_SUCCESS = "{0} đặt lại mật khẩu thành công";
            public const string CHANGEPASS_SUCCESS = "{0} đổi mật khẩu thành công";
            public const string GETDATA_SUCCESS = "{0} truy xuất dữ liệu thành công";
            public const string CREATE_SUCCESS = "{0} thêm thành công {1} dữ liệu mới";
            public const string UPDATE_SUCCESS = "{0} cập nhật dữ liệu thành công";
            public const string DELETE_SUCCESS = "{0} xóa dữ liệu thành công";
            public const string UPLOADFILE_SUCCESS = "{0} đã tải lên thành công {1} tệp tin";
            // Failed details message
            public const string ADDQUESTIONS_FROMEXCEL_FAILED = "{0} tải lên câu hỏi từ Excel thất bại";
            public const string UPLOADEXISTEDFILE = "{0} tải lên tệp tin đã tồn tại!";
            public const string LOGINFAIL = "{0} đăng nhập thất bại! ";
            public const string GETFAIL = "{0} truy xuất dữ liệu thất bại! ";
            public const string CREATEFAIL = "{0} thêm dữ liệu thất bại! ";
            public const string UPDATEFAIL = "{0} cập nhật dữ liệu thất bại! ";
            public const string DELETEFAIL = "{0} xóa dữ liệu thất bại! ";
        }
        public class NameFormat
        {
            public const string AVATAR_NAMEFORMAT = "{0}_Avatar_{1}"; /* {UserName}_Avatar_{Time} */
            public const string ATTACHMENT_NAMEFORMAT = "{0}/{1}"; /* Skill/Time */
        }
        public class QuestionConstant
        {
            public const double ContentSimilarityCoefficient = 0.8;
            public const double AttachmentSimilarityCoefficient = 0.75;
            public const double MAX_AUDIO_DURATION_IN_SECONDS = 480;

            public const int StatusReviewing = 0;
            public const int StatusReady = 1;
            public const int StatusDuplicate = 2;

            public const string MULTIPLECHOICE_QUESTION_SHEETNAME = "TRẮC NGHIỆM";
            public const string ESSAY_QUESTION_SHEETNAME = "TỰ LUẬN";

            public const string NoData = "Không có dữ liệu phù hợp!";
            public const string GetFailed = "Lấy dữ liệu thất bại!";

            public const string QUESTION_TYPE_NOTSUPPORT = "Kỹ năng này không dành cho câu hỏi trắc nghiệm!";
            public const string ESSAYQUESTION_HAS_MUTIPLE_ANSWERS = "Câu hỏi tự luận không được có nhiều hơn một đáp án!";

            public const string TRUEANSWER_REQUIRED = "Câu hỏi phải có đáp án đúng!";

            public const string NOT_ENOUGH_2OPTIONS = "Câu hỏi trắc nghiệm phải có ít nhất hai đáp án!";
            public const string ANSWER_HAS_MORE_THAN_4OPTIONS = "Câu hỏi trắc nghiệm chỉ có thể có tối đa 4 đáp án!";
            public const string ANSWERS_MUST_NOT_BE_DUPLICATE = "Đáp án của câu hỏi trắc nghiệm không được trùng lặp!";

            public const string AUDIOFILE_REQUIRED = "Câu hỏi nghe phải đi kèm file nghe!";
            public const string AUDIOFILE_EMPTY = "File nghe không đủ độ dài!";

            public const string CONTENT_DUPLICATED = "Câu hỏi trong hệ thống có nội dung tương tự với câu hỏi mới!";
            public const string ATTACHMENT_DUPLICATED = "Câu hỏi trong hệ thống có tệp đính kèm tương tự câu hỏi mới!";
            public const string BOTH_DUPLICATED = "Câu hỏi trong hệ thống có cả nội dung và tệp đính kèm tương tự câu hỏi mới!";

            public const string NOTCOMPATIBLE_FILE = "Tập tin không tương thích với loại câu hỏi!";
            public const string UNSUPPORTED_FILE = "Hệ thống không hỗ trợ loại tập tin này!";
            public const string FileNotFound = "Không tìm thấy tập tin!";
            public const string FileExisted = "Tập tin đã tồn tại!";

            public const string QUESTIONTYPE_NOTFOUND = "Không tìm thấy loại câu hỏi tương ứng!";
            public const string ACCESSLOG_NOTFOUND = "Có lỗi trong quá trình thêm câu hỏi, lịch sử nhập câu hỏi không tồn tại!";
            public const string USER_NOTFOUND = "Không tìm thấy người dùng!";
            public const string LEVEL_NOTFOUND = "Không tìm thấy trình độ!";
            public const string TOPIC_NOTFOUND = "Không tìm thấy chủ đề!";
            public const string REFERENCE_NOTFOUND = "Tham chiếu của câu hỏi không tồn tại!";
            public const string TASK_NOTFOUND = "Nhiệm vụ không tồn tại!";
            public const string TASK_ATTEMPTING_FAIL = "Không thể làm nhiệm vụ của người khác!";


            public const string AddMultipleSuccess = "Thêm thành công {0} câu hỏi! ";
            public const string AddSuccess = "Thêm câu hỏi thành công! ";
            public const string AddFailed = "Thêm câu hỏi thất bại! ";


            public const string TableRegexLisRead = @"^(정답|첨부파일|질문|[A-Z])$";
            public const string TableRegexSpeWri = @"^(첨부파일|질문)$";
            public const string NullDataTable = "Câu hỏi thứ {0} là một bảng trống.";
            public const string NullTable = "File không nội dung.";
            public const string DuplicateTable = "Câu hỏi thứ {0} trùng lặp với câu hỏi thứ {1}.";
            public const string HasSpecialCharacter = "Câu hỏi thứ {0} có ký tự đặc biệt.";
            public const string HasNullLeftCell = "Câu hỏi thứ {0} sai định dạng.";
            public const string HasQuestion = "질문";
            public const string HasNullQuestionContent = "Câu hỏi thứ {0} không có nội dung câu hỏi.";
            public const string HasAttachment = "첨부파일";
            public const string HasCorrectAnswer = "정답";
            public const string HasFalseImage = "Câu hỏi thứ {0} không được cung cấp tệp đính kèm.";
            public const string HasNullCorrectAnswer = "Câu hỏi thứ {0} không có 정답";
            public const string HasMoreCorrectAnswer = "Câu hỏi thứ {0} có nhiều hơn một 정답";
            public const string HasNulAttahment = "Câu hỏi thứ {0} không có 첨부파일";
            public const string HasMoreAttahment = "Câu hỏi thứ {0} có nhiều hơn một 첨부파일";
            public const string HasNullQuestion = "Câu hỏi thứ {0} không có 질문";
            public const string HasMoreQuestion = "Câu hỏi thứ {0} có nhiều hơn một 질문";
            public const string HasInvalidCorrectAnswer = "Câu hỏi thứ {0} có đáp án đúng không hợp lệ. Nếu có nhiều hơn 1 đáp án đúng, hãy phân chia bằng dấu phẩy (,)";
            public const string HasNotCorrectAnswer = "Câu hỏi thứ {0} không có đáp án đúng";
            public const string HasDuplicateAnswer = "Câu hỏi thứ {0} có đáp án giống nhau";
            public const string HasDuplicateChoice = "Câu hỏi thứ {0} có nhiều đáp án {1}";
            public const string HasNullChoice = "Câu hỏi thứ {0} có  đáp án {1} đang để trống";
            public const string HasDuplicateAttachment = "Tệp đính kèm có tên bị trùng lặp";
            public const string MustHaveSoundFile = "Vui lòng đính kèm file nghe";
            public const string MustHaveSoundFile1 = "Câu hỏi thứ {0} không có tên file nghe";
            public const string HaveWrongTableFormat = "Câu hỏi thứ {0} sai định dạng của bảng";
            public const string HaveMoreThan4Answers = "Câu hỏi thứ {0} có nhiều hơn 4 đáp án";


            public const string ADDQUESTION_METHOD_MANUAL = "Nhập thủ công";
            public const string ADDQUESTION_METHOD_WORDFILE = "Nhập bằng file word";
            public const string ADDQUESTION_METHOD_EXCELFILE = "Nhập bằng file excel";
        }
        public class QuestionTypeConstant
        {
            public const string AdminDeleted = "Đã xóa loại câu hỏi cùng câu hỏi và câu trả lời liên quan!";
            public const string Deleted = "Xóa thành công!";
            public const string FailedToDelete = "Không thể xóa dữ liệu này!";
        }
        public class LearningSection
        {
            public const string Empty = "Không tồn tại học phần nào!";
            public const string LevelNotExist = "Không tìm thấy trình độ học phần tương ứng!";

            public const string Added = "Thêm học phần mới thành công!";
            public const string Updated = "Chỉnh sửa học phần thành công!";
            public const string Deleted = "Xóa học phần thành công!";
        }
        public class AzureBlob
        {
            public const string IMPORTQUESTION_TEMPLATES_CONTAINER = "templatesimportquestionstorage";
            public const string MULTIPLECHOICE_QUESTION_TEMPLATE_FILENAME = "MultipleChoiceQuestionTemplate.xlsx";
            public const string ESSAY_QUESTION_IMPORTTEMPLATE_FILENAME = "EssayQuestionTemplate.xlsx";
            public const string IMPORT_QUESTION_WORD_TEMPLATE_FILENAME = "ImportQuestionWordTemplate.docx";
            public const string OPENENDEDQUESTION_IMPORTTEMPLATE_FILENAME = "OpenEndedQuestionTemplate.xlsx";

            public const string AzureBlobAvatarContainer = "fsakebavatarstorage";

            public const string AzureBlobListenAttachmentContainer = "attachlisteningstorage";
            public const string AzureBlobReadAttachmentContainer = "attachreadingstorage";
            public const string AzureBlobSpeakAndWriteAttachmentContainer = "attachspeakingandwritingstorage";

            public const string AzureBlobDefaultAvatar = "default_avatar.JPG";
            public const string SYSTEM_LOGO_URL = "https://fsakebfilestorage.blob.core.windows.net/fsakebavatarstorage/commonlogo.png";
        }
        public class FileType
        {
            public const string JPEGImage = "jpeg";
            public const string PNGImage = "png";
            public const string JPGImage = "jpg";
        }
        public static class Parameter
        {
            public const string Fail = "Hành động không thành công!";
            public const string NoContent = "Không tìm thấy!";
            public const string NotExist = "Mã tham số không tồn tại!!";

            public const string GenderCode = "GD";
            public const string GenderFemale = "GD-00";
            public const string GenderMale = "GD-01";
            public const string GenderUnknown = "GD-02";

            public const string LevelCode = "LV";
            public const string LevelBasic = "LV-01";
            public const string LevelInter = "LV-02";
            public const string LevelTopik3 = "LV-03";
            public const string LevelTopik4 = "LV-04";


            public const string SkillCode = "QS";
            public const string SkillListen = "QS-01";
            public const string SkillSpeak = "QS-02";
            public const string SkillRead = "QS-03";
            public const string SkillWrite = "QS-04";

            public const string LevelNotFound = "Trình độ không tồn tại trong hệ thống!";
            public const string LevelDuplicated = "Trình độ đã tồn tại trong hệ thống!";
            public const string LevelCreated = "Thêm trình độ mới thành công!";
            public const string LevelDeleted = "Xóa trình độ thành công!";
            public const string LevelUpdated = "Cập nhật trình độ thành công!";
            public const string LevelNameBlank = "Tên trình độ không được để trống!";

        }
        public static class Common
        {
            public const int ACTIVEALLOWEDTIME = 480; // This field is represented in minutes 
            public const string ACTIVEALLOWEDTIMEVALUE = "2 phút"; // This field is corresponding to the above

            public const string NEWACCOUNTSUBJECT = "TÀI KHOẢN MỚI TRÊN HỆ THỐNG FSAKEB";
            public const string AUTODEACTIVATE_SUBJECT = "FSAKEB: TỰ ĐỘNG KHÓA TÀI KHOẢN";
            public const string DEACTIVATESUBJECT = "FSAKEB: TÀI KHOẢN BỊ KHÓA";
            public const string REACTIVATESUBJECT = "FSAKEB: MỞ KHÓA TÀI KHOẢN!";
            public const string RESETPASSWORDSUBJECT = "FSAKEB: ĐẶT LẠI MẬT KHẨU!";

            public const string NEWACCOUTEMAIL = @"
                    <html>
                        <body>
                            <h1 style='color: blue;'>Gửi {0}!</h1>
                            <p>Bạn đã được cấp một tài khoản mới trên hệ thống FSAKEB! <bold style='color: red'>Vui lòng không tiết lộ thông tin đăng nhập cho người khác!!!</bold></p>
                            <p>Vì chính sách bảo mật, vui lòng đăng nhập và đổi mật khẩu của bạn trong vòng {1}! 
                            <bold style='color: red'>Lưu ý: hệ thống sẽ tự động khóa tài khoản sau thời gian trên!!!</bold>
                            </p>
                            <p style='color: red; font-size: 14px;'>Thông tin tài khoản:</p>
                            <p style='color: Green; font-size: 14px;'>
                                Email:<bold>{2}</bold>
                            </p>
                            <p><bold></bold></p>
                            <p>
                                <span style='color: Green; font-size: 14px;'>Tên đăng nhập: </span>
                                <bold style='text-decoration: italic;'>{3}</bold>
                            </p>
                            <p>
                                <span  style='color: Green; font-size: 14px;'>Mật khẩu: </span>
                                <bold style='text-decoration: underline;'>{4}</bold>
                            </p>
                            <p>Thân,<br/>FSAKEB Admin ~</p>
                        </body>
                    </html>";
            public const string AUTODEACTIVEEMAIL = @"
                    <html>
                        <body>
                            <h1 style='color: blue;'>Gửi {0}!</h1>
                            <p>
                                <bold style='color: red'>Vì bạn đã không đổi mật khẩu trong thời gian cho phép, hệ thống đã tự động khóa tài khoản của bạn!!!</bold>
                                Vui lòng liên hệ Admin để xử lý! 
                            </p>
                            <p>Thân,<br/>FSAKEB Admin ~</p>
                        </body>
                    </html>";
            public const string DEACTIVEEMAIL = @"
                    <html>
                        <body>
                            <h1 style='color: blue;'>Gửi, {0}!</h1>
                            <p>
                                <bold style='color: red'>Admin đã khóa tài khoản của bạn vì lý do đặc biệt!!!</bold>
                                Mọi thắc mắc xin vui lòng liên hệ Admin để xử lý! 
                            </p>
                            <p>Thân,<br/>FSAKEB Admin ~</p>
                        </body>
                    </html>";
            public const string REACTIVATEEMAIL = @"
                    <html>
                        <body>
                            <h1 style='color: blue;'>Gửi, {0}!</h1>
                            <p>Tài khoản của bạn đã được kích hoạt! 
                                <bold style='color: red'>Do now let anyone know your log in information!!!</bold>
                                <bold>Vì chính sách bảo mật, vui lòng đăng nhập và đổi mật khẩu của bạn trong vòng {1}! </bold>
                            </p>
                            <p style='color: Green; font-size: 14px;'>
                                Email: <bold>{2}</bold>
                            </p>
                            <p style='color: red; font-size: 14px;'><bold>Thông tin tài khoản:</bold></p>
                            <p>
                                <span style='color: Green; font-size: 14px;'>Tên đăng nhập: </span>
                                <bold style='text-decoration: italic;'>{3}</bold>
                            </p>
                            <p>
                                <span  style='color: Green; font-size: 14px;'>Mật khẩu: </span>
                                <bold style='text-decoration: underline;'>{4}</bold>
                            </p>
                            <p>Best regards,<br/>FSAKEB Admin ~</p>
                        </body>
                    </html>";
            public const string RESETPASSWORDEMAIL = @"
                        <html>
                            <body>
                                <h1 style='color: blue;'>Gửi {0}!</h1>
                                <p>Mật khẩu của bạn đã được đặt lại! Đừng cho ai biết nhé ~ 
                                <p>Vì chính sách bảo mật, vui lòng đăng nhập và đổi mật khẩu của bạn trong vòng {1}! 
                                <bold style='color: red'>Lưu ý: hệ thống sẽ tự động khóa tài khoản sau thời gian trên!!!</bold>
                                </p>
                                <p style='color: red; font-size: 14px;'>Thông tin tài khoản:</p>
                                <p style='color: Green; font-size: 14px;'>
                                    Email:<bold>{2}</bold>
                                </p>
                                <p><bold></bold></p>
                                <p>
                                    <span style='color: Green; font-size: 14px;'>Tên đăng nhập: </span>
                                    <bold style='text-decoration: italic;'>{3}</bold>
                                </p>
                                <p>
                                    <span  style='color: Green; font-size: 14px;'>Mật khẩu: </span>
                                    <bold style='text-decoration: underline;'>{4}</bold>
                                </p>
                                <p>Thân,<br/>FSAKEB Admin ~</p>
                            </body>
                        </html>";

            public const string PasswordNeedToBeChanged = "Bạn phải dổi mật khẩu cho lần đầu tiên đăng nhập!";
            public const string ItemNotExist = "Dữ liệu không tồn tại trong hệ thống!";
            public const string FailedToGet = "Truy xuất dữ liệu thất bại!";
            public const string FailedToCreate = "Thêm dữ liệu thất bại!";
            public const string FailedToUpdate = "Cập nhật dữ liệu thất bại!";
            public const string FailedToDelete = "Xóa dữ liệu thất bại!";
            public const string GetSuccess = "Lấy dữ liệu thành công! Kết quả gồm {0} bản ghi";
            public const string SUCCESS = "Thành công!";
            public const string Error = "Thất bại!";
            public const string ITEM_DUPLICATED = "Dữ liệu đã tồn tại!";
            public const string EmptyField = "{PropertyName} không được để trống!";
            public const string CommitFailed = "Hành động thất bại, vui lòng thử lại";

            // Added by naviank - 9/19/2024
            public const string PossibleCharsInPassword = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
                                                             + "abcdefghijklmnopqrstuvxyz"
                                                             + "0123456789"
                                                                //+ ".,/!@#$%^&*()_=+<>?;':[]{}"
                                                                ;
            public const string WrongLoginInfo = "Mật khẩu hoặc tài khoản hong đúng~";
            public const string WrongPassword = "Mật khẩu cũ không đúng!";
            public const string RequiredProperty = "Thuộc tính này không được để trống!";
            public const string RequiredEmail = "Email không được để trống!";

            public const string PasswordChanged = "Mật khẩu đã thay đổi!";
            public const string ConfirmPassNotMatch = "Mật khẩu xác nhận không khớp với mật khẩu mới";
            public const string WrongPasswordFormat = "Mật khẩu phải có ít nhất 8 ký tự, 1 ký tự in hoa, 1 chữ số và 1 ký tự đặc biệt";
            public const string NewPasswordIsNotNew = "Mật khẩu mới không được trùng với mật khẩu hiện tại";
            public const string Forbidden = "Tài khoản của bạn đã bị khóa";
            public const string ResetPassSuccessful = "Mật khẩu đã được cấp lại qua email, Xin vui lòng kiểm tra";

            public const string CreateAccountSuccess = "Tạo tài khoản thành công!";
        }
        public static class Validator
        {
            public const string IncorrectFormatField = "Sai định dạng {PropertyName} ({PropertyValue})";
            public const string NameIncorrectFormat = "Sai định dạng tên";
            public const string EmailIncorrectFormat = "Sai định dạng email";
            public const string PhoneNumberIncorrectFormat = "Sai định dạng số điện thoại";
            // Added by naviank - 9/19/2024
            public const string EmailExisted = "Email đã được đăng ký trong hệ thống";
            public const string UserUnder18 = "Ngày sinh không hợp lệ. Người dùng phải trên 18 tuổi";
            public const string EmailRegex = @"^[a-zA-Z0-9]+(\.[a-zA-Z0-9]+)*@[a-zA-Z0-9]+\.[a-zA-Z0-9]+(\.[a-zA-Z0-9]+)*$";
            public const string PasswordRegex = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,}$";
            public const string NameRegex = @"^([a-zA-Zà-ỹÀ-Ỹ]+\s*)+$";
        }
        public static class UserConstant
        {
            public const string NoChange = "Không có thay đổi!";
            public const string UpdatedFailed = "Thay đổi thông tin thất bại!";
            public const string UpdatedProfileSuccess = "Đã cập nhật thông tin người dùng!";
            public const string ChangedRole = "Đã thay đổi chức vụ người dùng!";
            public const string ChangedActiveStatus = "Đã thay đổi trạng thái hoạt động!";

        }
        public static class Permission
        {
            public const string NoPermission = "Bạn không có đủ quyền để thực hiện chức năng này! Vui lòng liên hệ Admin";
            public const string Updated = "Updated Permission {0}";
            public const string FailedToUpdate = "Failed to update Permission {0}";
            public const string Duplicated = "Permission are duplicated";
            public const string NotFound = "Permission is not found or deleted";
        }
        public static class Department
        {
            public const string DuplicatedDepartmentName = "Department name is duplicated";
            public const string NotFoundParentDepartment = "Parent Department is not found, deactivated or deleted";
            public const string SelftReferenced = "Your being updated Department is not allowed to set its parent as its children, try other ids";
        }
        public static class JobPosition
        {
            public const string EmptyField = "{PropertyName} is empty";
            public const string DuplicatedJobPositionName = "JobPosition name is duplicated";
        }
        public static class Main
        {
            public const string FromNotSoonerThanTo = "From must be sooner than To";
            public const string FromAndToMustBeInTheSameYear = "From and To must be in the same year";
            public const string FromAndToMustBeWithIn3Months = "From and To must be with in 3 months";
            public const string EmptyNhaHangList = "No nha hang";
            public const string ViewedListDimMain = "Viewed list dim main";
            public const string FailedToViewListDimMain = "Failed to view list dim main";
        }
        public static class FileConstant
        {
            public const string FailedToUpload = "Failed to upload file";
            public const string FileNameIsTooLong = "File name is too long";
            public const string NotAllowedExtensions = "File extension is not allowed";
            public const string NotAllowedContentTypes = "File content type is not allowed";
            public const string FileSizeIsTooLarge = "File size is too large";
            public const string VirusDetected = "Virus has been detected in your file";
            public const string Uploaded = "Uploaded {0} file(s)";
        }
        public static class Role
        {
            public const string Duplicated = "Role is duplicated";
            public const string NotFound = "Role is not found, deactivated  or deleted";
            public const string PermissionsRequired = "Role must contain at least one permission";

            public const string Updated = "Updated Role {0}";
            public const string FailedToUpdate = "Failed to update Role {0}";

            public const string Created = "Created Role {0}";
            public const string FailedToCreate = "Failed to create Role {0}";

            public const string Deleted = "Deleted Role {0}";
            public const string FailedToDelete = "Failed to delete Role {0}";
            public const string AlreadyDeleted = "Role is already deleted, you cannot delete it again";

            public const string Activated = "Activated Role {0}";
            public const string FailedToActivate = "Failed to activate Role {0}";
            public const string AlreadyActivated = "Role is already activated, you cannot activated it again";

            public const string Deactivated = "Deactivated Role {0}";
            public const string FailedToDeactivate = "Failed to deactivate Role {0}";
            public const string AlreadyDeactivated = "Role is already deactivated, you cannot deactivate it again";

            // Added by Naviank
            public const string AdminRoleId = "f089198c-ed4c-4294-9e62-ac9a09880000";
            public const string TeamLeadRoleId = "f089198c-ed4c-4294-9e62-ac9a09880001";
            public const string LecturerRoleId = "f089198c-ed4c-4294-9e62-ac9a09880002";
        }
        public static class Tenant
        {
            public const string Duplicated = "Tenant is duplicated";

            public const string Created = "Created Tenant {0}";
            public const string FailedToCreate = "Failed to create Tenant {0}";

            public const string Updated = "Updated Tenant {0}";
            public const string FailedToUpdate = "Failed to update Tenant {0}";

            public const string Deleted = "Deleted Tenant {0}";
            public const string FailedToDelete = "Failed to delete Tenant {0}";
            public const string AlreadyDeleted = "Tenant is already deleted, you cannot delete it again";

            public const string Activated = "Activated Tenant {0}";
            public const string FailedToActivate = "Failed to activate Tenant {0}";
            public const string AlreadyActivated = "Tenant is already activated, you cannot activate it again";

            public const string Deactivated = "Deactivated Tenant {0}";
            public const string FailedToDeactivate = "Failed to activate Tenant {0}";
            public const string AlreadyDeactivated = "Tenant is already deactivated, you cannot deactivate it again";
        }
        public static class PasswordValidation
        {
            public const string UniqueCharsField = "The number character of {PropertyName} must be at least {UniqueCharacterRequired} unique characters. You entered {TotalUniqueCharacter} unique characters";

            public const string NonAlphanumericField = "The {PropertyName} required special characters. Your password do not have special characters. Please try again";

            public const string LowerCaseField = "The {PropertyName} must have at least one lowercase ('a'-'z'). Please try again";
            public const string UpperCaseField = "The {PropertyName} must have at least one uppercase ('A'-'Z'). Please try again";
            public const string DigitField = "The {PropertyName} must have at least one digit ('0'-'9'). Please try again";
            public const string FailedToConfirmPassword = "Your confirm password is not same as your new password ";
        }
    }
}
