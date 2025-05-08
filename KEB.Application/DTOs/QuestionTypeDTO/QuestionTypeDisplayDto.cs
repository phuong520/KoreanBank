using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionTypeDTO
{
    public class QuestionTypeDisplayDto
    {
        public Guid QuestionTypeId { get; set; }
        public string QuestionTypeName { get; set; } = string.Empty;
        public string QuestionTypeContent { get; set; } = string.Empty;
        public Skill Skill { get; set; }
        public int NumOfQuestions { get; set; }

    }
}
