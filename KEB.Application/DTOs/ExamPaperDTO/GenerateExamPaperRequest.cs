using System;

namespace KEB.Application.DTOs.ExamPaperDTO
{
    public class GenerateExamPaperRequest
    {
        public Guid ExamId { get; set; }
        public Guid RequestedUserId { get; set; }
        public string IpAddress { get; set; }
    }
} 