using AutoMapper;
using KEB.Application.Services.Interfaces;
using KEB.Application.Utils;
using KEB.Domain.Enums;
using KEB.Infrastructure.UnitOfWorks;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KEB.Domain.ValueObject.LogicString;
using OfficeOpenXml;

namespace KEB.Application.Services.Implementations
{
    public class QuestionWithFileService : IQuestionWithFileService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public  QuestionWithFileService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<string> GetTemplateUrl(bool forMultipleChoice)
        {
            string? absolutePath = "";
            //try
            //{

            //    if (forMultipleChoice)
            //        absolutePath = await _unitOfWork.FileService.GetBlob(AzureBlob.MULTIPLECHOICE_QUESTION_TEMPLATE_FILENAME,
            //                                                AzureBlob.IMPORTQUESTION_TEMPLATES_CONTAINER);
            //    else
            //        absolutePath = await _unitOfWork.FileService.GetBlob(AzureBlob.ESSAY_QUESTION_IMPORTTEMPLATE_FILENAME,
            //                                                AzureBlob.IMPORTQUESTION_TEMPLATES_CONTAINER);
            //}
            //catch (Exception) { }
            return absolutePath + $"?timestamp={DateTime.UtcNow.Ticks}";
        }

        public async Task UploadExcelTemplate(string? message = "")
        {
            try
            {
                await UploadMultipleChoiceTemplate(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UploadMultipleChoiceTemplate: {ex.Message}");
            }

            try
            {
                await UploadEssayTemplate(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UploadEssayTemplate: {ex.Message}");
            }
        }
        private async Task UploadMultipleChoiceTemplate(string? message = "")
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage excel = new();
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
                multipleChoiceSheet.Column(4).Width = 28;
                {
                    multipleChoiceSheet.Cells[1, 0 + 1].Value = "NGUỒN";
                    multipleChoiceSheet.Cells[1, 1 + 1].Value = "CHỦ ĐỀ";
                    multipleChoiceSheet.Cells[1, 2 + 1].Value = "LOẠI CÂU HỎI";
                    multipleChoiceSheet.Cells[1, 3 + 1].Value = "ĐỘ KHÓ";
                    multipleChoiceSheet.Cells[1, 4 + 1].Value = "NỘI DUNG CÂU HỎI";
                    multipleChoiceSheet.Cells[1, 5 + 1].Value = "ĐÁP ÁN ĐÚNG";
                    multipleChoiceSheet.Cells[1, 6 + 1].Value = "ĐÁP ÁN 1 (BẮT BUỘC)";
                    multipleChoiceSheet.Cells[1, 7 + 1].Value = "ĐÁP ÁN 2 (BẮT BUỘC)";
                    multipleChoiceSheet.Cells[1, 8 + 1].Value = "ĐÁP ÁN 3";
                    multipleChoiceSheet.Cells[1, 9 + 1].Value = "ĐÁP ÁN 4";
                    multipleChoiceSheet.Cells[1, 10 + 1].Value = "TỆP ĐÍNH KÈM";
                }
                {
                    multipleChoiceSheet.Cells[2, 0 + 1].Value = "Tên nguồn tham khảo";
                    multipleChoiceSheet.Cells[2, 1 + 1].Value = "Chọn trình độ";
                    multipleChoiceSheet.Cells[2, 2 + 1].Value = "Chọn loại câu hỏi";
                    multipleChoiceSheet.Cells[2, 3 + 1].Value = "Chọn độ khó cho câu hỏi";
                    multipleChoiceSheet.Cells[2, 4 + 1].Value = "Nhập nội dung câu hỏi";
                    multipleChoiceSheet.Cells[2, 5 + 1].Value = "Đáp án đúng (vd: A / A,B / A,D)";
                    multipleChoiceSheet.Cells[2, 6 + 1].Value = "Nhập nội dung đáp án ở đây";
                    multipleChoiceSheet.Cells[2, 7 + 1].Value = "Nhập nội dung đáp án ở đây";
                    multipleChoiceSheet.Cells[2, 8 + 1].Value = "Nhập nội dung đáp án ở đây";
                    multipleChoiceSheet.Cells[2, 9 + 1].Value = "Nhập nội dung đáp án ở đây";
                    multipleChoiceSheet.Cells[2, 10 + 1].Value = "Dán tên tệp đính kèm (ví dụ: naviank.png) ở đây";
                }
                multipleChoiceSheet.Cells[1, 1, 1, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                multipleChoiceSheet.Cells[1, 1, 1, 11].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                multipleChoiceSheet.Cells[1, 1, 1, 11].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
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

            // Upload file to Azure
            using var memoryStream = new MemoryStream();
            excel.SaveAs(memoryStream);
            memoryStream.Position = 0;

            var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "excelFile", AzureBlob.MULTIPLECHOICE_QUESTION_TEMPLATE_FILENAME)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
            //var existedTemplate = await _unitOfWork.FileService.GetBlob(AzureBlob.MULTIPLECHOICE_QUESTION_TEMPLATE_FILENAME, AzureBlob.IMPORTQUESTION_TEMPLATES_CONTAINER);
            //if (!string.IsNullOrEmpty(existedTemplate))
            //{
            //    await _unitOfWork.FileService.DeleteBlob(AzureBlob.MULTIPLECHOICE_QUESTION_TEMPLATE_FILENAME, AzureBlob.IMPORTQUESTION_TEMPLATES_CONTAINER);
            //}
            //await _unitOfWork.FileService.Upload(AzureBlob.IMPORTQUESTION_TEMPLATES_CONTAINER, formFile, AzureBlob.MULTIPLECHOICE_QUESTION_TEMPLATE_FILENAME);
            //await _unitOfWork.AccessLogs.AddAsync(new Domain.Entities.SystemAccessLog
            //{
            //    AccessTime = DateTime.Now,
            //    ActionName = "Reupload excel template for importing questions",
            //    IsSuccess = true,
            //    TargetObject = "Import question excel template",
            //    Details = $"Reupload excel template for importing questions | {message}"
            //});
        }
        private async Task UploadEssayTemplate(string? message = "")
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage excel = new();
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
                {
                    essaySheet.Cells[1, 1].Value = "NGUỒN";
                    essaySheet.Cells[1, 2].Value = "CHỦ ĐỀ";
                    essaySheet.Cells[1, 3].Value = "LOẠI CÂU HỎI";
                    essaySheet.Cells[1, 4].Value = "ĐỘ KHÓ";
                    essaySheet.Cells[1, 5].Value = "NỘI DUNG CÂU HỎI";
                    essaySheet.Cells[1, 6].Value = "ĐÁP ÁN ĐÚNG";
                    essaySheet.Cells[1, 7].Value = "TỆP ĐÍNH KÈM";
                }
                {
                    essaySheet.Cells[2, 0 + 1].Value = "Tên nguồn tham khảo";
                    essaySheet.Cells[2, 1 + 1].Value = "Chọn trình độ";
                    essaySheet.Cells[2, 2 + 1].Value = "Chọn loại câu hỏi";
                    essaySheet.Cells[2, 3 + 1].Value = "Chọn độ khó cho câu hỏi";
                    essaySheet.Cells[2, 4 + 1].Value = "Nhập nội dung câu hỏi";
                    essaySheet.Cells[2, 5 + 1].Value = "Đáp án đúng (vd: A / A,B / A,D)";
                    essaySheet.Cells[2, 6 + 1].Value = "Dán tên tệp đính kèm (ví dụ: naviank.png) ở đây";
                }
                essaySheet.Cells[1, 1, 1, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                essaySheet.Cells[1, 1, 1, 7].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                essaySheet.Cells[1, 1, 1, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
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

            // Upload file to Azure
            using var memoryStream = new MemoryStream();
            excel.SaveAs(memoryStream);
            memoryStream.Position = 0;

            var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "excelFile", AzureBlob.ESSAY_QUESTION_IMPORTTEMPLATE_FILENAME)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
            //var existedTemplate = await _unitOfWork.FileService.GetBlob(AzureBlob.ESSAY_QUESTION_IMPORTTEMPLATE_FILENAME, AzureBlob.IMPORTQUESTION_TEMPLATES_CONTAINER);
            //if (!string.IsNullOrEmpty(existedTemplate))
            //{
            //    await _unitOfWork.FileService.DeleteBlob(AzureBlob.ESSAY_QUESTION_IMPORTTEMPLATE_FILENAME, AzureBlob.IMPORTQUESTION_TEMPLATES_CONTAINER);
            //}
            //await _unitOfWork.FileService.Upload(AzureBlob.IMPORTQUESTION_TEMPLATES_CONTAINER, formFile, AzureBlob.ESSAY_QUESTION_IMPORTTEMPLATE_FILENAME);
            //await _unitOfWork.AccessLogs.AddAsync(new Domain.Entities.SystemAccessLog
            //{
            //    AccessTime = DateTime.Now,
            //    ActionName = "Reupload excel template for importing questions",
            //    IsSuccess = true,
            //    TargetObject = "Import question excel template",
            //    Details = $"Reupload excel template for importing questions | {message}"
            //});
        }
    }
}
