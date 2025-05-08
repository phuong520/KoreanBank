using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionAddDTO
{
    public class ImportQuestionResultDTO
    {
        public object? Question { get; set; }
        public List<string> Messages { get; set; }

    }
}
