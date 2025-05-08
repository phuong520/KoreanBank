using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.LevelTopicDetailDTO
{
    public class NumOfQuestionsBySkillDTO
    {
        public int NumOfListeningQuestions { get; set; }
        public int NumOfSpeakingQuestions { get; set; }
        public int NumOfReadingQuestions { get; set; }
        public int NumOfWritingQuestions { get; set; }

    }
}
