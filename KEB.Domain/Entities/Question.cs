using KEB.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Entities
{
    public class Question : BaseEntity
    {
        public Difficulty Difficulty { get; set; }
        public string QuestionContent { get; set; }
        public ImageFile? AttachmentFileAudio { get; set; }
        public Guid? AttachFileAudioId { get; set; }
        public ImageFile? AttachmentFileImage { get; set; }
        public Guid? AttachFileImageId { get; set; }
        public int? AttachmentDuration { get; set; }
        public bool IsMultipleChoice { get; set; }
        public QuestionStatus Status { get; set; }
        public string? Description { get; set; } 
        public References References { get; set; }
        public Guid ReferenceId { get; set; }
        public QuestionType QuestionType { get; set; }
        public Guid QuestionTypeId { get; set; }
        public LevelDetail LevelDetail { get; set; }
        public Guid LevelDetailId { get; set; }
        public SystemAccessLog SystemAccessLog { get; set; }
        public Guid LogId { get; set; }
        public AddQuestionTask AddQuestionTask { get; set; }
        public Guid? TaskId { get; set; }
        public List<PaperDetail> PaperDetails { get; set; }
        public List<Answer> Answers { get; set; }

    }
}
