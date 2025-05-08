using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamTypeDTO
{
    public class ExamTypeGeneralDisplayDTO
    {
        public Guid ExamTypeId { get; set; }
        public string ExamTypeName { get; set; }
        public string LevelName { get; set; }
        public int OccuredExams { get; set; }

    }
}
