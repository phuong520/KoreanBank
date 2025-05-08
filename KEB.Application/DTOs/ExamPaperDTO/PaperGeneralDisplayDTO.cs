using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamPaperDTO
{
    public class PaperGeneralDisplayDTO
    {
        public Guid PaperId { get; set; }
        public string ExamName { get; set; }
        public string LevelName { get; set; }
        public string PaperName { get; set; }
        public DateTime TakePlaceTime { get; set; }
        public PaperStatus PaperStatus { get; set; }
        public string Skill { get; set; }

    }
}
