using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamTypeConstraintDTO
{
    public class InputExamTypeConstraintDetailDTO
    {
        public Guid TopicId { get; set; }
        public bool IsMultipleChoice { get; set; }
        public Guid QuestionTypeId { get; set; }
        public Difficulty Difficulty { get; set; }
        public int NumberOfQuestions { get; set; }
        public float MarkPerQuestion { get; set; }
        public Guid? ConstraintDetailId { get; set; }
        public Guid? ExamTypeConstraintId { get; set; }

    }
}
