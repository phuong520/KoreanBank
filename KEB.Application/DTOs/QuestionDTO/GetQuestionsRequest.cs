using KEB.Application.DTOs.Common;
using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionDTO
{
    public class GetQuestionsRequest
    {
        public Guid? CreatedBy { get; set; }
        public List<Guid>? LevelIds { get; set; }
        public List<Guid>? TopicIds { get; set; }
        public List<Guid>? ReferenceIds { get; set; } = [];
        public List<Guid>? QuestionTypeIds { get; set; } = [];
        public Skill? Skill { get; set; }
        public List<Difficulty>? Difficulties { get; set; } = [];
        public string SearchContent { get; set; } = string.Empty;
        public bool? IsMultipleChoice { get; set; }
        public List<QuestionStatus>? Status { get; set; } = [];
        public Guid? LogId { get; set; }
        public Guid? TaskId { get; set; }

        public DateTime FromTime { get; set; } = DateTime.MinValue;
        public DateTime ToTime { get; set; } = DateTime.Now;
        public bool SortAscending { get; set; } = true;
        public Pagination? PaginationRequest { get; set; } = new();

    }
}
