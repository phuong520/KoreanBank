using KEB.Application.DTOs.TopicDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.LevelDTO
{
    public class LevelDisplayDetailDTO
    {
        public Guid LevelId { get; set; }
        public string LevelName { get; set; }
        public List<TopicDisplayDto> Topics { get; set; } = [];

    }
}
