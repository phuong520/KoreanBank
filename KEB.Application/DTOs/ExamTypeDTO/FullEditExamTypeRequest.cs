using KEB.Application.DTOs.ExamTypeConstraintDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamTypeDTO
{
    public class FullEditExamTypeRequest
    {
        public Guid RequestedUserId { get; set; }
        public Guid TargetObjectId { get; set; }
        public Guid NewLevelId { get; set; }
        public string NewExamTypeName { get; set; }
        public bool ConstraintsChanged { get; set; }
        public List<InputExamTypeConstraintDTO> Constraints { get; set; }
        public string? IpAddress { get; set; }

    }
}
