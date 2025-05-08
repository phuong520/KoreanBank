using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Entities
{
    public class AddQuestionTask : BaseEntity
    {
        public string TaskName { get; set; }
        public bool ForMultiChoice { get; set; }
        public Difficulty Difficult { get; set; }
        public int NumberOfQuestion { get; set; }
        public string? ScheduleTaskId { get; set; }
        public DateTime DeadLine { get; set; }
        public AddQuestionStatus Status { get; set; }
        public QuestionType QuestionType { get; set; }
        public Guid QuestionTypeId { get; set; }
        public LevelDetail LevelDetail { get; set; }
        public Guid LevelDetailId { get; set; }
        public User User { get; set; }
        public Guid AssignId { get; set; }
        public List<Question> Questions { get; set; }
    }
}
