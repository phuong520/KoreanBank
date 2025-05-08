using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionDTO
{
    public class ChangeQuestionStatusRequest
    {
        public Guid RequestedUserId { get; set; }
        public Guid LogId { get; set; }
        public Guid NotifyTo { get; set; }
        public List<SingleChangeQuestionStatusRequest> Requests { get; set; }
        public string? IpAddress { get; set; }

    }
}
