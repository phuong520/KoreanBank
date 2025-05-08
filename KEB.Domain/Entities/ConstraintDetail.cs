using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Entities
{
    public class ConstraintDetail: BaseEntity
    {
        public int NumberOfQuestion { get; set; }
        public Difficulty Difficulty { get; set; }
        public float MarkPerQuestion { get; set; }
        public bool IsMultipleChoice { get; set; }
        public Topic Topic { get; set; }
        public Guid TopicId { get; set; }
        public QuestionType QuestionType { get; set; }
        public Guid QuestionTypeId { get; set; }
        public ExamTypeConstraint ExamTypeConstraint { get; set; }
        public Guid ExamTypeConstraintId { get; set; }
    }
}
