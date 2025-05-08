using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamPaperDTO
{
    public class PaperChangeDTO
    {
        public Guid OldQuestionId { get; set; }
        public Guid NewQuestionId { get; set; }

    }
}
