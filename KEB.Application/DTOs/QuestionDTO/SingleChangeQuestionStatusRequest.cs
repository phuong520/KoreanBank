using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionDTO
{
    public class SingleChangeQuestionStatusRequest
    {
        public Guid QuestionId { get; set; }
        public string? Reason { get; set; }
        public QuestionStatus ToStatus { get; set; }
    }
}
