using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ExamPaperDTO
{
    public class EditPaperDetailRequestVer2
    {
        public Guid RequestedUserId { get; set; }
        public Guid PaperId { get; set; }
        public List<PaperChangeDTO> Changes { get; set; }
        public bool Changed { get; set; } = false;
        public string? IpAddress { get; set; }

    }
}
