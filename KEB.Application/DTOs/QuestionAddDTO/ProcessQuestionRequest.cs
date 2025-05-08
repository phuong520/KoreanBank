using KEB.Application.DTOs.AnswerDTO;
using KEB.Application.DTOs.QuestionTypeDTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionAddDTO
{
    public class ProcessQuestionRequest
    {
        public Guid RequestedUserId { get; set; }
        public string QuestionContent { get; set; }
        public string TopicName { get; set; }
        public IEnumerable<AnswerDTO.AddAnswerDTO> Answers { get; set; } = [];
        public QuestionTypeDisplayDto QuestionType { get; set; }
        public IFormFile? Attachment { get; set; }
        public string? AttachmentHashedContent { get; set; } = string.Empty;
        public int DurationInSeconds { get; set; } = 0;
        public bool DoManually { get; set; }
    }
}
