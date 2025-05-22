using KEB.Application.DTOs.AnswerDTO;
using KEB.Domain.Entities;
using KEB.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionAddDTO
{
    public class AddSingleQuestionRequest
    {
        public Guid LevelDetailId { get; set; }
        public Guid ReferenceId { get; set; }
        public Guid QuestionTypeId { get; set; }
        public Guid RequestedUserId { get; set; }
        public Difficulty Difficulty { get; set; }
        public string? QuestionContent { get; set; }
        public List<AddAnswerDTO>? Answers { get; set; } = new();
        public IFormFile? AttachmentFileImage { get; set; }
        public IFormFile? AttachmentFileAudio { get; set; }
        public int? AttachmentDuration { get; set; } = 0;
        public bool IsMultipleChoice { get; set; } = true;
        public Guid? TaskId { get; set; }
        public Skill Skill { get; set; }

    }

}

