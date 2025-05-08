using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.ImportQuestionTaskDTO
{
    public class SumUpImportedQuestionsDTO
    {
        public string LevelDetail { get; set; }
        public string QuestionTypeName { get; set; }
        public string QuestionForm { get; set; }
        public string Difficulty { get; set; }
        public string Skill { get; set; }
        public int TotalQuestions { get; set; }
        public int ApprovedQuestions { get; set; }

    }
}
