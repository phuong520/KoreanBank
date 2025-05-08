using KEB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionAddDTO
{
    public class AddQuestionRequest
    {
        public User RequestedUser { get; set; }
        public List<AddSingleQuestionDTO> Requests { get; set; } = [];
        public string? IpAddress { get; set; } = "::1";
        //public string? AddMethod { get; set; } = "Nhập thủ công";
        public Guid? TaskId { get; set; }

    }
}
