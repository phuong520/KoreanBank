using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.LevelTopicDetailDTO
{
    public class DeleteDetailRequest
    {
        public Guid RequestedUserId { get; set; }
        public Guid LevelId { get; set; }
        public Guid TopicId { get; set; }
        public string? IpAddress { get; set; }

    }
}
