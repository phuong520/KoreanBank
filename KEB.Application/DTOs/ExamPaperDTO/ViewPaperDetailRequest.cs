using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamPaperDTO
{
    public class ViewPaperDetailRequest
    {
        public Guid RequestedUserId { get; set; }
        public Guid PaperId { get; set; }
        public string IpAddress { get; set; }

    }
}
