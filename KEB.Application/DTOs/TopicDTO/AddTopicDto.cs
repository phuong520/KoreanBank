using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.TopicDTO
{
    public class AddTopicDto
    {
        public Guid CreatedBy { get; set; }
        public string TopicName { get; set; }
        public string Description { get; set; }
        public string? IpAddress { get; set; }
        public List<Guid>? Levels { get; set; } = [];

    }
}
