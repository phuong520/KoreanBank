using KEB.Application.DTOs.LevelTopicDetailDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.TopicDTO
{
    public class TopicDisplayDto
    {
        public Guid TopicId { get; set; }
        public string TopicName { get; set; }
        public string Description { get; set; }
        public int NumOfRelatedLevels { get; set; }
        public NumOfQuestionsBySkillDTO NumOfQuestions { get; set; }

    }
}
