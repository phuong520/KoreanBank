using KEB.Application.DTOs.AnswerDTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionDTO
{
    public class QuestionDisplayDto
    {
        public Guid Id { get; set; }
        public string QuestionContent { get; set; } = string.Empty;
        public string? Status { get; set; }
        public string TopicName { get; set; }
        public string LevelName { get; set; }
        public string ReferenceName { get; set; }
        public Guid QuestionTypeId { get; set; }
        public string QuestionTypeName { get; set; }
        public string SkillName { get; set; }
        public string Difficulty { get; set; }
        public IFormFile? AttachmentUrl { get; set; }
        public IEnumerable<AnswerDTO.AddAnswerDTO> Answers { get; set; }
        public int OrderInPaper { get; set; }
        public string Description { get; set; }

    }
}
