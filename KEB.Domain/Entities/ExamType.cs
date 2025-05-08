using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Entities
{
    public class ExamType: BaseEntity
    {
        public string  ExamTypeName { get; set; }
        public Level Levels { get; set; }
        public Guid LevelId { get; set; }
        public List<Exam> Exams { get; set; }
        public List<ExamTypeConstraint> ExamTypeConstraints {  get; set; }
    }
}
