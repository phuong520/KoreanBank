using KEB.Application.DTOs.LevelTopicDetailDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.LevelDTO
{
    public class LevelDisplayBriefDTO
    {
        public Guid LevelId { get; set; }
        public string LevelName { get; set; }
        public List<DetailDisplayDTO> Details { get; set; }
        public int NumOfRelatedTopics { get; set; }
        public NumOfQuestionsBySkillDTO NumOfQuestions { get; set; }

    }
}
