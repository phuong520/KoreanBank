using KEB.Application.DTOs.LevelDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.TopicDTO
{
    public class TopicDetailDto
    {
        public Guid TopicId { get; set; }
        public string TopicName { get; set; }
        public string Description { get; set; }
        public List<LevelDisplayBriefDTO> Levels { get; set; }

    }

}
