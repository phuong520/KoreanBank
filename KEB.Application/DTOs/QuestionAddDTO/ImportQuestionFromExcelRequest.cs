using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionAddDTO
{
    public class ImportQuestionFromExcelRequest
    {
        public Guid RequestedUserId { get; set; }
        public IFormFile ExcelFile { get; set; }
        public bool ForMultipleChoice { get; set; } = true;
        public Guid? TaskId { get; set; }
        public IEnumerable<IFormFile> Attachments { get; set; } = [];

    }
}
