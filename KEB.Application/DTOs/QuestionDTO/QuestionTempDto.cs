using KEB.Application.DTOs.AnswerDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionDTO
{
    public class QuestionTempDto
    {
        public string QuestionContent { get; set; }
        public string? AttachmentUrl { get; set; }
        public IEnumerable<AnswerDTO.AddAnswerDTO> Answers { get; set; }

    }
}
