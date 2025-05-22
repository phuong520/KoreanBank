using KEB.Application.DTOs.AnswerDTO;
using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamPaperDTO
{
    public class QuestionInPaperDTO
    {
        public Guid Id { get; set; }
        public string QuestionContent { get; set; } = string.Empty;
        public string TopicName { get; set; }
        public string LevelName { get; set; }
        public string ReferenceName { get; set; }
        public Guid QuestionTypeId { get; set; }
        public string QuestionTypeName { get; set; }
        public string QuestionTypeContent { get; set; }
        public Skill Skill { get; set; }
        public string Difficulty { get; set; }
        public string? AttachmentImage { get; set; }
        public string? AttachmentAudio { get; set; }
        public IEnumerable<AddAnswerDTO> Answers { get; set; }
        public float Mark { get; set; }
        public int OrderInPaper { get; set; }

    }
}
