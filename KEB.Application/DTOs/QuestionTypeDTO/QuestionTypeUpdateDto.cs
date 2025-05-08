using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionTypeDTO
{
    public class QuestionTypeUpdateDto
    {
        public Guid QuestionTypeId { get; set; }
        public Guid UpdatedUserId { get; set; }
        public string QuestionTypeName { get; set; }
        public string QuestionTypeContent { get; set; }

    }
}
