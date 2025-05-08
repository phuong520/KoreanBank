using KEB.Application.DTOs.ExamTypeConstraintDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamTypeDTO
{
    public class ExamTypeComplexDisplayDTO
    {
        public Guid ExamTypeId { get; set; }
        public Guid LevelId { get; set; }
        public string LevelName { get; set; }
        public string ExamTypeName { get; set; }
        public List<ConstraintToBeDisplayedDTO> PaperConstraints { get; set; } = [];
        public int OccuredExams { get; set; }

    }
}
