using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.SystemAccessLogDTO
{
    public class AddQuestionHistoryDto
    {
        public Guid Id { get; set; }
        public string TaskName { get; set; }
        public string UserName { get; set; }
        public DateTime AccessTime { get; set; }
        public string ActionName { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int ApprovedQuestions { get; set; }
        public int NeedReviewQuestions { get; set; }
        public int DeniedQuestions { get; set; }
        public int DuplicatedQuestions { get; set; }

    }
}
