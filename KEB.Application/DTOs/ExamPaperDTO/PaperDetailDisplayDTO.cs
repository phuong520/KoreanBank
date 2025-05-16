using KEB.Application.DTOs.ExamTypeConstraintDTO;
using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamPaperDTO
{
    public class PaperDetailDisplayDTO
    {
        public Guid PaperId { get; set; }
        public string PaperName { get; set; }
        public Guid ExamId { get; set; }
        public string ExamName { get; set; }
        public string LevelName { get; set; }
        public DateTime TakePlaceTime { get; set; }
        public ConstraintToBeDisplayedDTO PaperConstraint { get; set; }
        public List<QuestionInPaperDTO> QuestionsList { get; set; } = [];
        public PaperStatus PaperStatus { get; set; }
        public bool Reviewed { get; set; }
        public Guid HostId { get; set; }
        public Guid ReviewerId { get; set; }

    }
}
