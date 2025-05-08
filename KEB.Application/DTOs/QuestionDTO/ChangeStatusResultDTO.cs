using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionDTO
{
    public class ChangeStatusResultDTO
    {
        public Guid QuestionId { get; set; }
        public string Action { get; set; }
        public string QuestionContent { get; set; }

    }
}
