using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ImportQuestionTaskDTO
{
    public class ImportQuestionTaskDetail
    {
        public Guid LevelDetailId { get; set; }
        public Guid QuestionTypeId { get; set; }
        public bool ForMultipleChoice { get; set; }
        public Difficulty Difficulty { get; set; }
        public int NumberOfQuestions { get; set; }
        public DateTime Deadline { get; set; }
        public string? Message { get; set; }

    }

}
