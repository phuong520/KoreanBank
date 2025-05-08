using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Entities
{
    public class QuestionType :BaseEntity
    {
        public string TypeName { get; set; }
        public string TypeContent { get; set; }
        public Skill Skill {  get; set; }
        public List<ConstraintDetail> ConstraintDetails { get; set; }
        public List<Question> Questions { get; set; }
        public List<AddQuestionTask> AddQuestions { get; set; }
    }
}
