using KEB.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamPaperDTO
{
    public class ViewExamPapersListRequest
    {
        public Guid? ExamId { get; set; }
        public Guid? LevelId { get; set; }
        public string? NameValueToBeSearched { get; set; }
        public DateTime? LowerTakePlaceTimeBound { get; set; }
        public Pagination PaginationRequest { get; set; }

    }
}
