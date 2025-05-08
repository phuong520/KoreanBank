using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Entities
{
    public class ExamTypeConstraint: BaseEntity
    {
        public Skill Skill {  get; set; }
        public int TotalNumberOfQuestions { get; set; }
        public int DurationInMinutes { get; set; }
        public int NumberOfPaper {  get; set; }
        public ExamType ExamType { get; set; }
        public Guid ExamTypeId { get; set; }
        public List<ConstraintDetail> ConstraintDetails { get; set; } = new List<ConstraintDetail>();
    }
}
