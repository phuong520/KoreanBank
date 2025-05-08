using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.LevelTopicDetailDTO
{
    public class AddValuesToEntityRequest
    {
        public Guid RequestedUserId { get; set; }
        public Guid TargetObjectId { get; set; }
        public List<Guid> Values { get; set; }
        public string? IpAddress { get; set; }

    }
}
