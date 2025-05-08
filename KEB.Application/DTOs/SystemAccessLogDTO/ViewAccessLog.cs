using KEB.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.SystemAccessLogDTO
{
    public class ViewAccessLog
    {
        public Guid? UserId { get; set; }
        public bool? IsSuccess { get; set; }
        public string TargetObject { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public DateTime? From { get; set; } = DateTime.MinValue;
        public DateTime? To { get; set; } = DateTime.Now;
        public Pagination? PaginationRequest { get; set; } = new();

    }
}
