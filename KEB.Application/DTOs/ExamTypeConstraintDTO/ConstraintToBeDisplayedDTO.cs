using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamTypeConstraintDTO
{
    public class ConstraintToBeDisplayedDTO
    {
        public Guid ExamTypeConstraintId { get; set; }
        public Skill Skill { get; set; }
        public int TotalNumberOfQuestions { get; set; }
        public int DurationInMinutes { get; set; }
        public int NumberOfPapers { get; set; }
        public List<ConstraintDetailToBeDisplayedDTO?> ConstraintDetails { get; set; } = [];

    }
}
