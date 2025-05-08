using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.Common
{
    public class Delete
    {
        public Guid RequestedUserId { get; set; }
        public Guid TargetObjectId { get; set; }
        public string? IpAddress { get; set; }
        public bool HardDelete { get; set; } = true;

    }
}
