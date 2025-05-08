using KEB.Application.DTOs.Common;
using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ImportQuestionTaskDTO
{
    public class ViewTaskRequest
    {
        public Guid? AssigneeId { get; set; }
        public Guid? LevelId { get; set; }
        public Guid? QuestionTypeId { get; set; }
        public AddQuestionStatus? Status { get; set; }
        public DateTime? LowerBound { get; set; }
        public DateTime? UpperBound { get; set; }
        public Pagination? PaginationRequest { get; set; } = new();

    }
}
