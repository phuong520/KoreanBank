using KEB.Application.DTOs.SystemAccessLogDTO;
using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ImportQuestionTaskDTO
{
    public class TaskFullDisplayDTO
    {
        public Guid Id { get; set; }
        public string TaskName { get; set; }
        public Guid AssigneeId { get; set; }
        public string AssigneeName { get; set; }
        public DateTime Deadline { get; set; }
        public string LevelDetailId { get; set; }
        public string LevelDetail { get; set; }
        public string QuestionTypeId { get; set; }
        public string QuestionTypeName { get; set; }
        public Skill Skill { get; set; }
        public bool ForMultipleChoice { get; set; }
        public Difficulty Difficulty { get; set; }
        public int NumberOfQuestions { get; set; }
        public AddQuestionStatus Status { get; set; }
        public List<SumUpImportedQuestionsDTO> SumUp { get; set; } = [];
        public List<AddQuestionHistoryDto> ImportHistory { get; set; } = [];

    }
}
