using KEB.Application.DTOs.AnswerDTO;
using KEB.Domain.Entities;
using KEB.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionAddDTO
{
    public class AddSingleQuestionDTO
    {
        public LevelDetail LevelDetail { get; set; }
        public References Reference { get; set; }
        public QuestionType QuestionType { get; set; }
        public User RequestedUser { get; set; }

        public Difficulty Difficulty { get; set; }
        public string QuestionContent { get; set; }
        public List<AddAnswerDTO>? Answers { get; set; }
        public IFormFile? Attachment { get; set; }
        public int AttachmentDuration { get; set; } = 0;
        public bool IsMultipleChoice { get; set; } = true;

        public AddQuestionTask? Task { get; set; }
        public SystemAccessLog AccessLog { get; set; }

    }
}
