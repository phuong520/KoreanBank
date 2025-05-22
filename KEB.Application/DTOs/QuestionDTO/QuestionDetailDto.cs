using KEB.Application.DTOs.AnswerDTO;
using KEB.Application.DTOs.UserDTO;
using KEB.Domain.Entities;
using KEB.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionDTO
{
    public class QuestionDetailDto
    {
        public Guid Id { get; set; }
        public string QuestionContent { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid LevelId { get; set; }
        public string LevelName { get; set; }
        public Guid TopicId { get; set; }
        public string TopicName { get; set; }
        public Difficulty Difficulty { get; set; }
        public Guid ReferenceId { get; set; }
        public string ReferenceName { get; set; }
        public Guid QuestionTypeId { get; set; }
        public string QuestionTypeName { get; set; }
        public string SkillName { get; set; }
        public string? Status { get; set; }

        public IFormFile? AttachmentUrl { get; set; }
        public string? AttachmentImage { get; set; }
        public string? AttachmentAudio { get; set; }
        public IEnumerable<AddAnswerDTO> Answers { get; set; }

        public ShortUserDTO? CreatedUser { get; set; }
        public DateTime CreatedTime { get; set; }

        public ShortUserDTO? UpdatedUser { get; set; }
        public DateTime UpdatedTime { get; set; }

    }
}
