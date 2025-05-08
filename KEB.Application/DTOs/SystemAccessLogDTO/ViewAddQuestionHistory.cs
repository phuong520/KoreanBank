using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.SystemAccessLogDTO
{
    public class ViewAddQuestionHistory
    {
        public Guid? UserId { get; set; }
        public Guid? TaskId { get; set; }
        public DateTime? StartDate { get; set; } = DateTime.MinValue;
        public string? Action { get; set; } = "tạo mới";

    }
}
