using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamDTO
{
    public class AddExamRequest
    {
        public Guid RequestedUserId { get; set; }
        public Guid ExamTypeId { get; set; }
        public string ExamName { get; set; }
        public DateTime TakePlaceTime { get; set; }
        public Guid HostId { get; set; }
        public Guid ReviewerId { get; set; }
        public string? IpAddress { get; set; }

    }
}
