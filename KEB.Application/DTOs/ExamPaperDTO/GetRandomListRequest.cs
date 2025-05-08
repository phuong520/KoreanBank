using KEB.Domain.Entities;
using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamPaperDTO
{
    public class GetRandomListRequest
    {
        public ConstraintDetail ConstraintDetail { get; set; }
        public Skill Skill { get; set; }
        public Guid LevelId { get; set; }
        public Guid ExamId { get; set; }

    }
}
