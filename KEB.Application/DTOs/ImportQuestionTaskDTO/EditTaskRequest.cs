using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ImportQuestionTaskDTO
{
    public class EditTaskRequest
    {
        public Guid RequestedUserId { get; set; }
        public Guid TaskId { get; set; }
        public Guid? AssigneeId { get; set; }
        public Guid? LevelDetailId { get; set; }
        public Guid? QuestionTypeId { get; set; }
        public bool? ForMultipleChoice { get; set; }
        public Difficulty? Difficulty { get; set; }
        public int? NumberOfQuestions { get; set; }
        public DateTime? Deadline { get; set; }
        public string? IpAddress { get; set; }

    }
}
