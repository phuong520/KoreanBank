using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamDTO
{
    public class ExamGeneralDisplayDTO
    {
        public Guid ExamId { get; set; }
        public string ExamTypeName { get; set; }
        public string Examname { get; set; }
        public string LevelName { get; set; }
        public DateTime TakePlaceTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool IsSuspended { get; set; }
        public string Occured { get; set; }

    }
}
