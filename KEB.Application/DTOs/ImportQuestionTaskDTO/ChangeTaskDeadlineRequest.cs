using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ImportQuestionTaskDTO
{
    public class ChangeTaskDeadlineRequest
    {
        public Guid RequestedUserId { get; set; }
        public Guid TargetTaskId { get; set; }
        public DateTime NewDeadLine { get; set; }
        public string? IpAddress { get; set; }

    }
}
