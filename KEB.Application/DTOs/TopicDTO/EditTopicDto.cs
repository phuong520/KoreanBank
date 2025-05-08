using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.TopicDTO
{
    public class EditTopicDto
    {
        public Guid CreatedBy { get; set; }
        public Guid TopicId { get; set; }
        public string NewTopicName { get; set; }
        public string NewDescription { get; set; }
        public string? IpAddress { get; set; }

    }
}
