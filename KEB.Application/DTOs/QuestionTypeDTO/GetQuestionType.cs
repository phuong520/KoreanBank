using KEB.Application.DTOs.Common;
using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionTypeDTO
{
    public class GetQuestionType
    {
        public string? NameValueToBeSearched { get; set; } = string.Empty;
        public Skill? Skill { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? FromTime { get; set; }
        public bool? IsDeleted { get; set; }
        public Pagination? PaginationRequest { get; set; } = new();

    }
}
