using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Entities
{
    public class Answer : BaseEntity
    {
        public string AnswerContent { get; set; }
        public bool IsTrue { get; set; }
        public Question Question { get; set; }
        public Guid QuestionId { get; set; }
    }
}
