using KEB.Application.DTOs.ExamTypeConstraintDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamTypeDTO
{
    public class AddExamTypeRequest
    {
        //public Guid? ExamTypeId { get; set; }
        public Guid RequestedUserId { get; set; }
        public Guid LevelId { get; set; }
        public string ExamTypeName { get; set; }
        public List<InputExamTypeConstraintDTO> ExamTypeConstraints { get; set; }
        public string? IpAddress { get; set; }

    }
}
