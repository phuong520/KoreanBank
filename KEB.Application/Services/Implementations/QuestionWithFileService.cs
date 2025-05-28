using AutoMapper;
using KEB.Application.Services.Interfaces;
using KEB.Application.Utils;
using KEB.Domain.Enums;
using KEB.Infrastructure.UnitOfWorks;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml.Style;
using static KEB.Domain.ValueObject.LogicString;
using OfficeOpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace KEB.Application.Services.Implementations
{
    public class QuestionWithFileService : IQuestionWithFileService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _templatePath;
        private readonly string _uploadPath;
        public QuestionWithFileService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _templatePath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "templates");
            _uploadPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "uploads");
            Directory.CreateDirectory(_templatePath);
            Directory.CreateDirectory(_uploadPath);
        }

        public async Task<string> GetTemplateUrl(bool forMultipleChoice)
        {
            var fileName = forMultipleChoice ? "multiple_choice_template.xlsx" : "essay_template.xlsx";
            var filePath = Path.Combine(_templatePath, fileName);

            if (!File.Exists(filePath))
            {
                if (forMultipleChoice)
                    await UploadMultipleChoiceTemplate();
                else
                    await UploadEssayTemplate();
            }

            // Return relative URL (adjust based on your hosting setup)
            return $"/templates/{fileName}?timestamp={DateTime.UtcNow.Ticks}";
        }

        public async Task<byte[]> UploadExcelTemplate(bool forMultipleChoice = true, string ? message = "")
        {
            try
            {
                return await (forMultipleChoice ? UploadMultipleChoiceTemplate(message) : UploadEssayTemplate(message));
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to generate Excel template: {ex.Message}", ex);
                // Hoặc trả về Array.Empty<byte>() nếu không muốn ném exception:
                // return Array.Empty<byte>();
            }
        }
        private async Task<byte[]> UploadMultipleChoiceTemplate(string? message = "")
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //ExcelPackage.License.SetLicense(LicenseContext.NonCommercial);

            using var excel = new ExcelPackage();
            var multipleChoiceSheet = excel.Workbook.Worksheets.Add("TRẮC NGHIỆM");
            var typeSheet = excel.Workbook.Worksheets.Add("LOẠI CÂU HỎI");
            var levelSheet = excel.Workbook.Worksheets.Add("CHỦ ĐỀ");
            var refSheet = excel.Workbook.Worksheets.Add("NGUỒN THAM KHẢO");
            ExcelUtils.SetFontSizeAndDefaultCellSizeForExcel(excel, 12, 24, 32);
            multipleChoiceSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            // Header
            //// Multiple choice questions header
            {
                multipleChoiceSheet.Cells.Style.WrapText = true;
                multipleChoiceSheet.Row(1).Style.Font.Bold = true;
                multipleChoiceSheet.Row(1).Style.Font.Size = 12;
                multipleChoiceSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                multipleChoiceSheet.Row(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                multipleChoiceSheet.Row(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                multipleChoiceSheet.Column(5).Width = 28;
                multipleChoiceSheet.Cells[1, 1].Value = "NGUỒN";
                multipleChoiceSheet.Cells[1, 2].Value = "CHỦ ĐỀ";
                multipleChoiceSheet.Cells[1, 3].Value = "LOẠI CÂU HỎI";
                multipleChoiceSheet.Cells[1, 4].Value = "ĐỘ KHÓ";
                multipleChoiceSheet.Cells[1, 5].Value = "NỘI DUNG CÂU HỎI";
                multipleChoiceSheet.Cells[1, 6].Value = "ĐÁP ÁN ĐÚNG";
                multipleChoiceSheet.Cells[1, 7].Value = "ĐÁP ÁN 1 (BẮT BUỘC)";
                multipleChoiceSheet.Cells[1, 8].Value = "ĐÁP ÁN 2 (BẮT BUỘC)";
                multipleChoiceSheet.Cells[1, 9].Value = "ĐÁP ÁN 3";
                multipleChoiceSheet.Cells[1, 10].Value = "ĐÁP ÁN 4";
                multipleChoiceSheet.Cells[1, 11].Value = "TỆP ÂM THANH";
                multipleChoiceSheet.Cells[1, 12].Value = "TỆP HÌNH ẢNH";
                multipleChoiceSheet.Cells[2, 1].Value = "Tên nguồn tham khảo";
                multipleChoiceSheet.Cells[2, 2].Value = "Chọn trình độ";
                multipleChoiceSheet.Cells[2, 3].Value = "Chọn loại câu hỏi";
                multipleChoiceSheet.Cells[2, 4].Value = "Chọn độ khó cho câu hỏi";
                multipleChoiceSheet.Cells[2, 5].Value = "Nhập nội dung câu hỏi";
                multipleChoiceSheet.Cells[2, 6].Value = "Đáp án đúng (vd: A / A,B / A,D)";
                multipleChoiceSheet.Cells[2, 7].Value = "Nhập nội dung đáp án ở đây";
                multipleChoiceSheet.Cells[2, 8].Value = "Nhập nội dung đáp án ở đây";
                multipleChoiceSheet.Cells[2, 9].Value = "Nhập nội dung đáp án ở đây";
                multipleChoiceSheet.Cells[2, 10].Value = "Nhập nội dung đáp án ở đây";
                multipleChoiceSheet.Cells[2, 11].Value = "Dán tên tệp âm thanh (ví dụ: audio.mp3) ở đây";
                multipleChoiceSheet.Cells[2, 12].Value = "Dán tên tệp hình ảnh (ví dụ: image.png) ở đây";
                multipleChoiceSheet.Cells[1, 1, 1, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                multipleChoiceSheet.Cells[1, 1, 1, 12].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                multipleChoiceSheet.Cells[1, 1, 1, 12].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }
            //// Reference header
            {
                refSheet.Row(1).Style.Font.Bold = true;
                refSheet.Row(1).Style.Font.Size = 12;
                refSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                refSheet.Cells[1, 2].Value = "NGUỒN THAM KHẢO";
            }
            //// Question types header
            {
                typeSheet.Row(1).Style.Font.Bold = true;
                typeSheet.Row(1).Style.Font.Size = 12;
                typeSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                typeSheet.Cells[1, 2].Value = "LOẠI CÂU HỎI (Nghe đọc)";
                typeSheet.Cells[1, 3].Value = "LOẠI CÂU HỎI (All)";
            }
            //// Levels header
            {
                levelSheet.Row(1).Style.Font.Bold = true;
                levelSheet.Row(1).Style.Font.Size = 12;
                levelSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                levelSheet.Cells[1, 2].Value = "CHỦ ĐỀ";
            }

            // Values
            //// Reference values
            {
                int refIndex = 1;
                var refList = await _unitOfWork.References.GetAllAsync();
                foreach (var item in refList)
                {
                    refSheet.Cells[refIndex + 1, 2].Value = $"{item.ReferenceName}-{item.PublishedYear}";
                    refIndex++;
                }
                ExcelUtils.AddValidation(
                    multipleChoiceSheet,
                    "A2:A247",
                    $"'NGUỒN THAM KHẢO'!$B$2:$B${refIndex + 1}",
                    "Invalid Option",
                    "Please select a valid reference from the list."
                );
            }
            //// Question type values
            {
                int typeIndex = 1;
                var typeList = await _unitOfWork.QuestionTypes.GetAllAsync(x => x.Skill == Skill.Nghe || x.Skill == Skill.Đọc);
                foreach (var type in typeList)
                {
                    typeSheet.Cells[typeIndex + 1, 2].Value = $"{type.Skill}-{type.TypeName}";
                    typeIndex++;
                }
                ExcelUtils.AddValidation(
                    multipleChoiceSheet,
                    "C2:C247",
                    $"'LOẠI CÂU HỎI'!$B$2:$B${typeIndex + 1}",
                    "Invalid Option",
                    "Please select a valid level from the list."
                );
            }
            //// Level-Topic values
            {
                int levelIndex = 1;
                var levelList = await _unitOfWork.LevelDetails.GetAllAsync(includeProperties: "Level,Topic");
                foreach (var level in levelList)
                {
                    levelSheet.Cells[levelIndex + 1, 2].Value = $"{level.Level.LevelName}-{level.Topic.TopicName}";
                    levelIndex++;
                }
                ExcelUtils.AddValidation(
                    multipleChoiceSheet,
                    "B2:B247",
                    $"'CHỦ ĐỀ'!$B$2:$B${levelIndex + 1}",
                    "Invalid Option",
                    "Please select a valid level from the list."
                );
            }
            //// Difficulty validation
            {
                ExcelUtils.AddValidation(
                    sheet: multipleChoiceSheet,
                    cellRange: "D2:D247",
                    errorMessageTitle: "Invalid Option",
                    errorMessage: "Please select a valid level from the list.",
                    Difficulty.Dễ.ToString(),
                    Difficulty.Vừa.ToString(),
                    Difficulty.Khó.ToString()
                );
            }

            using var memoryStream = new MemoryStream();
            await excel.SaveAsAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream.ToArray();

        }
        private async Task<byte[]> UploadEssayTemplate(string? message = "")
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var excel = new ExcelPackage();
            var essaySheet = excel.Workbook.Worksheets.Add("TỰ LUẬN");
            var typeSheet = excel.Workbook.Worksheets.Add("LOẠI CÂU HỎI");
            var levelSheet = excel.Workbook.Worksheets.Add("CHỦ ĐỀ");
            var refSheet = excel.Workbook.Worksheets.Add("NGUỒN THAM KHẢO");
            ExcelUtils.SetFontSizeAndDefaultCellSizeForExcel(excel, 12, 24, 32);
            essaySheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            // Header
            //// Essay questions header
            {
                essaySheet.Cells.Style.WrapText = true;
                essaySheet.Row(1).Style.Font.Bold = true;
                essaySheet.Row(1).Style.Font.Size = 12;
                essaySheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                essaySheet.Column(4).Width = 28;

                essaySheet.Cells.Style.WrapText = true;
                essaySheet.Row(1).Style.Font.Bold = true;
                essaySheet.Row(1).Style.Font.Size = 12;
                essaySheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                essaySheet.Column(5).Width = 28;
                essaySheet.Cells[1, 1].Value = "NGUỒN";
                essaySheet.Cells[1, 2].Value = "CHỦ ĐỀ";
                essaySheet.Cells[1, 3].Value = "LOẠI CÂU HỎI";
                essaySheet.Cells[1, 4].Value = "ĐỘ KHÓ";
                essaySheet.Cells[1, 5].Value = "NỘI DUNG CÂU HỎI";
                essaySheet.Cells[1, 6].Value = "ĐÁP ÁN ĐÚNG";
                essaySheet.Cells[1, 7].Value = "TỆP ÂM THANH";
                essaySheet.Cells[1, 8].Value = "TỆP HÌNH ẢNH";
                essaySheet.Cells[2, 1].Value = "Tên nguồn tham khảo";
                essaySheet.Cells[2, 2].Value = "Chọn trình độ";
                essaySheet.Cells[2, 3].Value = "Chọn loại câu hỏi";
                essaySheet.Cells[2, 4].Value = "Chọn độ khó cho câu hỏi";
                essaySheet.Cells[2, 5].Value = "Nhập nội dung câu hỏi";
                essaySheet.Cells[2, 6].Value = "Nhập đáp án mẫu";
                essaySheet.Cells[2, 8].Value = "Dán tên tệp hình ảnh (ví dụ: image.png) ở đây";
                essaySheet.Cells[1, 1, 1, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                essaySheet.Cells[1, 1, 1, 8].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                essaySheet.Cells[1, 1, 1, 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                //// Reference header
                {
                    refSheet.Row(1).Style.Font.Bold = true;
                    refSheet.Row(1).Style.Font.Size = 12;
                    refSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    refSheet.Cells[1, 2].Value = "NGUỒN THAM KHẢO";
                }
                //// Question types header
                {
                    typeSheet.Row(1).Style.Font.Bold = true;
                    typeSheet.Row(1).Style.Font.Size = 12;
                    typeSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    typeSheet.Cells[1, 2].Value = "LOẠI CÂU HỎI (Nghe đọc)";
                    typeSheet.Cells[1, 3].Value = "LOẠI CÂU HỎI (All)";
                }
                //// Levels header
                {
                    levelSheet.Row(1).Style.Font.Bold = true;
                    levelSheet.Row(1).Style.Font.Size = 12;
                    levelSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    levelSheet.Cells[1, 2].Value = "CHỦ ĐỀ";
                }

                // Values
                //// Reference values
                {
                    int refIndex = 1;
                    var refList = await _unitOfWork.References.GetAllAsync();
                    foreach (var item in refList)
                    {
                        refSheet.Cells[refIndex + 1, 2].Value = $"{item.ReferenceName}-{item.PublishedYear}";
                        refIndex++;
                    }
                    ExcelUtils.AddValidation(
                        essaySheet,
                        "A2:A247",
                        $"'NGUỒN THAM KHẢO'!$B$2:$B${refIndex + 1}",
                        "Invalid Option",
                        "Please select a valid reference from the list."
                    );
                }
                //// Question types values
                {
                    int typeIndex = 1;
                    var typeList = await _unitOfWork.QuestionTypes.GetAllAsync();
                    //Essay sheet
                    foreach (var type in typeList)
                    {
                        typeSheet.Cells[typeIndex + 1, 3].Value = $"{type.Skill}-{type.TypeName}";
                        typeIndex++;
                    }
                    ExcelUtils.AddValidation(
                        essaySheet,
                        "C2:C247",
                        $"'LOẠI CÂU HỎI'!$C$2:$C${typeIndex + 1}",
                        "Invalid Option",
                        "Please select a valid level from the list."
                    );
                }
                //// Level-Topic values
                {
                    int levelIndex = 1;
                    var levelList = await _unitOfWork.LevelDetails.GetAllAsync(includeProperties: "Level,Topic");
                    foreach (var level in levelList)
                    {
                        levelSheet.Cells[levelIndex + 1, 2].Value = $"{level.Level.LevelName}-{level.Topic.TopicName}";
                        levelIndex++;
                    }
                    ExcelUtils.AddValidation(
                        essaySheet,
                        "B2:B247",
                        $"'CHỦ ĐỀ'!$B$2:$B${levelIndex + 1}",
                        "Invalid Option",
                        "Please select a valid level from the list."
                    );
                }
                //// Difficulty validation
                {
                    ExcelUtils.AddValidation(
                        sheet: essaySheet,
                        cellRange: "D2:D247",
                        errorMessageTitle: "Invalid Option",
                        errorMessage: "Please select a valid level from the list.",
                        Difficulty.Dễ.ToString(),
                        Difficulty.Vừa.ToString(),
                        Difficulty.Khó.ToString()
                    );
                }

                using var memoryStream = new MemoryStream();
                await excel.SaveAsAsync(memoryStream);
                memoryStream.Position = 0;
                return memoryStream.ToArray();
            }
        }

       
    }
}
