using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamDTO
{
    public class GetExamAsTaskRequest
    {
        public Guid? UserId { get; set; }
        public Guid? LevelId { get; set; }
        public bool? Occured { get; set; }
        public bool? Host { get; set; }

    }
}
