using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ImportQuestionTaskDTO
{
    public class AssignTaskRequest
    {
        public Guid RequestedUserId { get; set; }
        public Guid AssigneeId { get; set; }
        public List<ImportQuestionTaskDetail> TasksList { get; set; } = [];
        public string? IpAddress { get; set; }

    }
}
