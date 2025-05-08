using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionTypeDTO
{
    public class QuestionTypeCreateDto
    {
        public string QuestionTypeName { get; set; }
        public string QuestionTypeContent { get; set; }
        public Skill Skill { get; set; }
        public Guid CreatedUserId { get; set; }

    }
}
