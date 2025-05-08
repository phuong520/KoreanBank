using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamDTO
{
    public class EditExamRequest
    {
        public Guid RequestedUserId { get; set; }
        public Guid TargetObjectId { get; set; }
        public Guid? NewExamTypeId { get; set; }
        public string? NewExamName { get; set; }
        public DateTime? NewTakePlaceTime { get; set; }
        public Guid? NewHostId { get; set; }
        public Guid? NewReviewerId { get; set; }
        public string? IpAddress { get; set; }

    }
}
