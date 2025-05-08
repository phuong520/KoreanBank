using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionDTO
{
    public class GetDuplicateQuestionResultDTO
    {
        public QuestionDetailDto Question { get; set; }
        public string Message { get; set; }

    }
}
