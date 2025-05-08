using KEB.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionAddDTO
{
    public class ImportQuestionByWordRequest
    {
        public Guid RequestedUserId { get; set; }
        public Guid LevelDetailId { get; set; }
        public Guid QuestionTypeId { get; set; }
        public Guid ReferenceId { get; set; }
        public Difficulty Difficulty { get; set; }
        public Guid? TaskId { get; set; }
        public string? IpAddress { get; set; }
        public IFormFile WordAttachment { get; set; }
        public IEnumerable<IFormFile>? Attachment { get; set; }

    }
}
