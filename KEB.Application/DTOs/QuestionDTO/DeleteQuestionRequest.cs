using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionDTO
{
    public class DeleteQuestionRequest
    {
        public Guid QuestionId { get; set; }
        public Guid RequestedUserId { get; set; }
        public string? IpAddress { get; set; }

    }
}
