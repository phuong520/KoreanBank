using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ImportQuestionTaskDTO
{
    public class RemindTaskDeadlineRequest
    {
        public Guid TaskId { get; set; }
        public string AssigneeName { get; set; }
        public string AssigneeEmail { get; set; }
        public string TaskName { get; set; }
        public DateTime Deadline { get; set; }
        public string TaskDetails { get; set; }

    }
}
