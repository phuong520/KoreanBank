using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamDTO
{
    public class ExamAsTaskDisplayDTO
    {
        public Guid ExamId { get; set; }
        public string ExamName { get; set; }
        public string LevelName { get; set; }
        public DateTime TakePlaceTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public string UserName { get; set; }
        public string TaskName { get; set; }
        public bool Occured { get; set; }

    }
}
