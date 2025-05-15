using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamTypeConstraintDTO
{
    public class ConstraintDetailToBeDisplayedDTO
    {
        public Guid ConstraintDetailId { get; set; }
        public Guid QuestionTypeId { get; set; }
        public string QuestionTypeName { get; set; }
        public string QuestionForm { get; set; }
        public Guid TopicId { get; set; }
        public string TopicName { get; set; }
        public Difficulty Difficulty { get; set; }
        public int NumOfQuestions { get; set; }
        public float MarkPerQuestion { get; set; }
        public float TotalMark { get; set; }

    }
}
