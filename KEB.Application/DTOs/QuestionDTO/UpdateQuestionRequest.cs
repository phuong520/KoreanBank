using KEB.Application.DTOs.AnswerDTO;
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
    public class UpdateQuestionRequest
    {
        public Guid RequestedUserId { get; set; }
        public Guid TargetObjectId { get; set; }
        public string? NewQuestionContent { get; set; }
        public List<AddAnswerDTO>? Answers { get; set; } = new List<AddAnswerDTO>();
        public bool AnswersChanged { get; set; } = false;
        //public IFormFile? NewAttachment { get; set; }
        public IFormFile? AttachmentFileImage { get; set; }
        public IFormFile? AttachmentFileAudio { get; set; }
        public bool AttachmentChanged { get; set; } = false;
        public Difficulty? NewDifficulty { get; set; }
        public Guid? NewReferenceId { get; set; }
        public string? IpAddress { get; set; }

    }
}
