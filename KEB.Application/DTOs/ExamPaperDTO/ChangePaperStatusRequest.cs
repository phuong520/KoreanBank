using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamPaperDTO
{
    public class ChangePaperStatusRequest
    {
        public Guid RequestedUserId { get; set; }
        public Guid PaperId { get; set; }
        public PaperStatus NewStatus { get; set; }
        public string? IpAddress { get; set; }

    }
}
