using KEB.Application.DTOs.QuestionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionAddDTO
{
    public class MultipleImportResultDTO
    {
        public List<string> Errors { get; set; } = new List<string>();
        public List<QuestionTempDto> Results { get; set; } = [];

    }
}
