using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamPaperDTO
{
    public class LeaveCommentRequest
    {
        public Guid RequestedUserId { get; set; }
        public Guid TargetObjectId { get; set; }
        public string Content { get; set; }
        public string? IpAddress { get; set; }

    }
}
