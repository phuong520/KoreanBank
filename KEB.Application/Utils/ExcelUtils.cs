using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KEB.Domain.ValueObject.LogicString;

namespace KEB.Application.Utils
{
    public class ExcelUtils
    {
        public static void AddValidation(ExcelWorksheet sheet, string cellRange, string formulaRange, string errorMessageTitle, string errorMessage)
        {
            var validation = sheet.DataValidations.AddListValidation(cellRange);
            validation.Formula.ExcelFormula = formulaRange;
            validation.ShowErrorMessage = true;
            validation.ErrorTitle = errorMessageTitle;
            validation.Error = errorMessage;
        }
        public static void AddValidation(ExcelWorksheet sheet, string cellRange, string errorMessageTitle, string errorMessage, params string[] values)
        {
            var validation = sheet.DataValidations.AddListValidation(cellRange);
            foreach (var value in values)
            {
                validation.Formula.Values.Add(value);
            }
            validation.ShowErrorMessage = true;
            validation.ErrorTitle = errorMessageTitle;
            validation.Error = errorMessage;
        }
        public static void SetFontSizeAndDefaultCellSizeForExcel(ExcelPackage package, int fontSize, int defaultRowHeight, int defaultColumnWidth, string sheetName = "")
        {
            if (!string.IsNullOrEmpty(sheetName))
            {
                var worksheet = package.Workbook.Worksheets[sheetName] ?? throw new Exception(string.Format(ExceptionMessage.SHEET_NAME_NOTFOUND, sheetName));
                SetFontSizeAndDefaultCellSizeForSheet(worksheet, fontSize, defaultRowHeight, defaultColumnWidth);
            }
            else
            {
                foreach (var worksheet in package.Workbook.Worksheets)
                    SetFontSizeAndDefaultCellSizeForSheet(worksheet, fontSize, defaultRowHeight, defaultColumnWidth);
            };
        }
        public static void SetFontSizeAndDefaultCellSizeForSheet(ExcelWorksheet worksheet, int fontSize, int defaultRowHeight, int defaultColumnWidth)
        {
            worksheet.DefaultRowHeight = defaultRowHeight;
            worksheet.DefaultColWidth = defaultColumnWidth;
            worksheet.Cells.Style.Font.Size = fontSize;
        }

        public static void SetWrapTextAndCustomHeight(ExcelPackage package, bool wraptext, bool customHeight, string sheetName = "")
        {
            if (!string.IsNullOrEmpty(sheetName))
            {
                var worksheet = package.Workbook.Worksheets[sheetName] ?? throw new Exception(string.Format(ExceptionMessage.SHEET_NAME_NOTFOUND, sheetName));
                SetWrapTextAndCustomHeight(worksheet, wraptext, customHeight);
            }
            else
            {
                foreach (var worksheet in package.Workbook.Worksheets)
                    SetWrapTextAndCustomHeight(worksheet, wraptext, customHeight);
            };
        }

        public static void SetWrapTextAndCustomHeight(ExcelWorksheet worksheet, bool wraptext, bool customHeight)
        {
            worksheet.Cells.Style.WrapText = wraptext;
            for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
            {
                worksheet.Row(i).CustomHeight = customHeight;
            }
        }

        public static void SetBorderForContentRange(ExcelWorksheet worksheet)
        {
            var contentRange = worksheet.Cells[worksheet.Dimension.Address];
            contentRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            contentRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            contentRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            contentRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }
    }
}
