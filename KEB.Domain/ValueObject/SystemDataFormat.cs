using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.ValueObject
{
    public class SystemDataFormat
    {
        public const int QUESTION_STORING_DURATION_AFTER_IMPORTING = 7; // 7 days


        public const int NUMOFHOURS_BEFORE_DEADLINE_TO_ALERT = 12;
        public const int MIN_TIME_TO_ASSIGN_TASK_IN_DAYS = 1;

        public const string ADD_QUESTION_TASK_NAMEFORMAT = "{0}_문제 추가하는 임무_{1}_{2}";
        public const string TASK_MULTIPLECHOCIE_INFO_FORMAT = @"
                    <li style='background-color: #f9f9f9; padding: 10px; border: 1px solid #FFFFFF; border-radius: 4px;'>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <tr>
                                <td colspan='2' style='padding: 5px; color: red;'><strong>Tên nhiệm vụ:</strong> {0}</td>
                                <td colspan='1' style='padding: 5px; color: red;'><strong>Deadline:</strong> {1}</td>
                            </tr>
                            <tr>
                                <td style='padding: 5px 0px;'><strong>Chủ đề:</strong> {2}</td>
                                <td style='padding: 5px 0px;'><strong>Độ khó:</strong> {3}</td>
                                <td style='padding: 5px 0px;'><strong>Số lượng:</strong> {4} câu trắc nghiệm</td>
                            </tr>
                            <tr>
                                <td colspan='1' style='padding: 5px;'><strong>Kỹ năng:</strong> {5}</td>
                                <td colspan='2' style='padding: 5px;'><strong>Loại câu hỏi:</strong> {6}</td>
                            </tr>
                        </table>
                    </li>";

        public const string TASK_ESSAY_INFO_FORMAT = @"
                    <li style='background-color: #f9f9f9; padding: 10px; border: 1px solid #FFFFFF; border-radius: 4px;'>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <tr>
                                <td colspan='2' style='padding: 5px; color: red;'><strong>Tên nhiệm vụ:</strong> {0}</td>
                                <td colspan='1' style='padding: 5px; color: red;'><strong>Deadline:</strong> {1}</td>
                            </tr>
                            <tr>
                                <td style='padding: 5px 0px;'><strong>Chủ đề:</strong> {2}</td>
                                <td style='padding: 5px 0px;'><strong>Độ khó:</strong> {3}</td>
                                <td style='padding: 5px 0px;'><strong>Số lượng:</strong> {4} câu tự luận</td>
                            </tr>
                            <tr>
                                <td colspan='1' style='padding: 5px;'><strong>Kỹ năng:</strong> {5}</td>
                                <td colspan='2' style='padding: 5px;'><strong>Loại câu hỏi:</strong> {6}</td>
                            </tr>
                        </table>
                    </li>";

        public const string INFORM_NEWTASK_EMAIL_BODY = @"
                    <html>
                        <body>
                            <h2 style='color: blue;'>Gửi {0}!</h2>
                            <p>{1} vừa giao cho bạn {2} nhiệm vụ mới! <bold>Vui lòng đăng nhập vào hệ thống KEB để kiểm tra!!!</bold></p>
                            <div> 
                            <h4>Chi tiết nhiệm vụ:</h4>
                            <ul style='list-style-type: decimal;'>
                                {3}
                            </ul>
                            <em> ~ Đừng trễ deadline nhé ~ </em>
                            <h4>Chúc bạn làm việc vui vẻ ~ !!!</h4>
                            </div>
                            <p>Thân,<br/>PhuongHT40 ~</p>
                        </body>
                    </html>";

        public const string INFORM_UPDATETASK_EMAIL_BODY = @"
                    <html>
                        <body>
                            <h2 style='color: blue;'>Gửi {0}!</h2>
                            <p>{1} vừa chỉnh sửa nhiệm vụ {2} của bạn! <bold>Vui lòng đăng nhập vào hệ thống FSAKEB để kiểm tra!!!</bold></p>
                            <div> 
                                <h4>Chi tiết nhiệm vụ:</h4>
                                <div><bold>Ban đầu:</bold>{3}</div>
                                <div><bold>Được sửa thành:</bold>{4}</div>
                                <em> ~ Đừng trễ deadline nhé ~ </em>
                                <h4>Chúc bạn làm việc vui vẻ ~ !!!</h4>
                            </div>
                            <p>Thân,<br/>Naviank ~</p>
                        </body>
                    </html>";

        public const string INFORM_REMOVEACCESS_FROMTASK_EMAIL_BODY = @"
                    <html>
                        <body>
                            <h2 style='color: blue;'>Gửi {0}!</h2>
                            <p>Người dùng {1} vừa đá bạn khỏi nhiệm vụ {2}! <bold> Đăng nhập vào hệ thống FSAKEB hoặc liên hệ admin để biết chi tiết!!!</bold></p>
                            <div> 
                                <p>Nhiệm vụ mới giờ đã được giao cho {3}!!!</p>
                                <h4>Chúc bạn làm việc vui vẻ ~ !!!</h4>
                            </div> 
                            <p>Thân,<br/>Naviank ~</p>
                        </body>
                    </html>";

        public const string INFORM_DEADLINE_APPROACHING_EMAIL_BODY = @"
                    <html>
                        <body>
                            <h2 style='color: blue;'>Gửi {0}!</h2>
                            <p>Thời hạn cho phép cho nhiệm vụ {1} của bạn sẽ kết thúc vào {2}! <bold> Hãy thử kiểm tra lại nhé!!!</bold></p>
                            <div> 
                                <h4>Chi tiết nhiệm vụ:</h4>
                                <div>{3}</div>
                                <h4>Chúc bạn làm việc vui vẻ ~ Đừng để lead buồn!!!</h4>
                            </div> 
                            <p>Thân,<br/>Naviank ~</p>
                        </body>
                    </html>";
        //// Đoạn này là email subject
        public const string INFORM_DEADLINE_APPROACHING_EMAIL_SUBJECT = "KEB - THỜI HẠN CHO NHIỆM VỤ CỦA BẠN SẮP HẾT";

        public const string INFORM_NEWTASK_EMAIL_SUBJECT = "KEB - BẠN VỪA NHẬN ĐƯỢC NHIỆM VỤ MỚI";

        public const string INFORM_UPDATETASK_EMAIL_SUBJECT = "KEB - NHIỆM VỤ CỦA BẠN CÓ THAY ĐỔI";

        public const string INFORM_REMOVEACCESS_FROMTASK_EMAIL_SUBJECT = "KEB - BẠN ĐÃ BỊ XÓA KHỎI NHIỆM VỤ";

        /// Từ phần này là cho feature Exam type
        public const int MINIMUM_DURATION_IN_MINUTES = 1;
        public const int MAXIMUM_NUMOFQUESTIONS_FOR_EACHPAPER = 100;
        public const int MINIMUM_NUMOFQUESTIONS_FOR_EACHPAPER = 2;
        public const int MAXIMUM_NUMOFQUESTIONS_FOR_EACHDETAIL_OF_PAPER = 10;


        // Từ phần này là cho feature Exam
        public const int EARLIEST_EXAM_TAKEPLACETIME_FROMNOW = 5;
        public const int DURATION_BEFORE_TAKEPLACETIME_CAN_DELETE = 3; // days
        public const int EXAM_INFO_EDIT_DURATION = 1; // day
        public const int EXAM_PAPERS_EDIT_DURATION = 2; // day

        public const string EXAM_NEWRELEVANCE_EMAIL_SUBJECT = "KEB - BẠN ĐÃ ĐƯỢC GIAO LÀM {0} CHO MỘT KỲ THI";
        public const string EXAM_NEWRELEVANCE_EMAIL_BODY = @"
                    <html>
                        <body>
                            <h2 style='color: blue;'>Gửi {0}!</h2>
                            <p>Người dùng {1} đã giao cho bạn làm {2} của kỳ thi {3}!</p>
                            <div> 
                                <h4>Chi tiết kỳ thi:</h4>
                                <table style='width: 100%; border-collapse: collapse;'>
                                    <tr>
                                        <td><strong>Loại kỳ thi:</strong> {4}</td>
                                        <td><strong>Tên kỳ thi:</strong> {5}</td>
                                    </tr>
                                    <tr>
                                        <td><strong>Trình độ:</strong> {6}</td>
                                        <td><strong>Ngày thi:</strong> {7}</td>
                                    </tr>
                                </table>
                                <a href=""https://vnexpress.net/"" 
                                   style=""display: inline-block; background-color: red; color: white; padding: 10px 20px; border: none; text-decoration: none; font-weight: bold; text-align: center;"">
                                   Xem chi tiết tại đây
                                </a>
                                <br/>
                                <h3>Chúc vui ~ </h3>
                            </div> 
                            <p>Thân,<br/>~ Naviank ~</p>
                        </body>
                    </html>";
        public const string EXAM_CHANGERELEVANCE_EMAIL_BODY = @"
                    <html>
                        <body>
                            <h2 style='color: blue;'>Gửi {0}!</h2>
                            <p>Bạn vừa được đổi vai trò từ {1} thành {2} của kỳ thi {3}!</p>
                            <div> 
                                <button style='background-color: red; color: white; padding: 10px 20px; border: none; 
                                            cursor: pointer;' onclick='location.href=\""https://vnexpress.net/\""'>
                                    Xem chi tiết tại đây
                                </button>
                                <br/>
                                <h3>Chúc vui ~ </h3>
                            </div> 
                            <p>Thân,<br/>~ Naviank ~</p>
                        </body>
                    </html>";
        public const string EXAM_KICKOUT_EMAIL_BODY = @"
                    <html>
                        <body>
                            <h2 style='color: blue;'>Gửi {0}!</h2>
                            <p>Người dùng {1} đã đá bạn khỏi kỳ thi {2}!</p>
                            <div> 
                                <h4>Liên hệ với KVan để biết thêm chi tiết</h4>
                                <button style='background-color: red; color: white; padding: 10px 20px; border: none; 
                                            cursor: pointer;' onclick='location.href=\""https://vnexpress.net/\""'>
                                    KVan's facebook link
                                </button>
                                <br/>
                                <h3>Chúc vui ~ </h3>
                            </div> 
                            <p>Thân,<br/>~ Naviank ~</p>
                        </body>
                    </html>";
        // Phần này là email gửi cho team lead sau khi có câu hỏi mới được thêm vào hệ thống
        public const string INFORM_NEWQUESTION_EMAIL_SUBJECT = "HỆ THỐNG CÓ CÂU HỎI MỚI CẦN ĐƯỢC BẠN REVIEW";
        public const string INFORM_NEWQUESTION_EMAIL_BODY = "{0} vừa thêm {1} câu hỏi mới vào hệ thống. Nhớ review sớm nhé ~";


        // Phần này là format cho đề thi
        public const string PAPER_CONTAINER_HTML_FORMAT = "" +
            "<div>" +
                "<div style=\"width: 100%; text-align:center\">" +
                    "<h2>{0}</h2>" +
                "</div>" +

                "<div>" +
                    "<div style=\"padding: 16px; width: 60%\">" +
                        "<img src=\"{1}\" alt=\"LTILogo\" style=\"height: 100px;\">" +
                    "</div>" +
                    "<div style=\"padding: 16px; width: 40%\">" +
                        "" +
                    "</div>" +
                "</div>" +
            "</div>";

        public const string PAPER_TITLE_HTML_FORMAT = "";
        public const string QUESTION_TYPE_HTML_FORMAT = "";
        public const string QUESTION_FLEXANSWER_HTML_FORMAT = "" +
            "<div class=\"question\">" +
                "<p class=\"question-content\"><span>Q{0}.</span> {1} <i>({2}점)</i></p>" +
                    "{3}" +
                    "<div class=\"answers1\">" +
                        "{4}" +
                    "</div>" +
            "</div>";
        public const string QUESTION_GRIDANSWER_HTML_FORMAT = "" +
            "<div class=\"question\">" +
                "<p class=\"question-content\"><span>Q{0}.</span> {1} <i>({2}점)</i></p>" +
                    "{3}" +
                    "<div class=\"answers2\">" +
                        "{4}" +
                    "</div>" +
            "</div>";
        public const string SINGLE_ANSWER_HTML_FORMAT = "<p> {0}. {1} </p>";


        public const string MCQ_FILL_ANSWER_HTML_FORMAT = "";
        public const string EQ_FILL_ANSWER_HTML_FORMAT = "";

    }
}
