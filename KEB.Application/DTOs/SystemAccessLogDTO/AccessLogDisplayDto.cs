using KEB.Application.DTOs.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.SystemAccessLogDTO
{
    public class AccessLogDisplayDto
    {
        public Guid Id { get; set; }
        public ShortUserDTO User { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public DateTime AccessTime { get; set; }
        public string TargetObject { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = true;

    }
}
