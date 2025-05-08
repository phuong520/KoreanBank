using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Entities
{
    public class LevelDetail: BaseEntity
    {
        public Topic Topic { get; set; }
        public Guid TopicId { get; set; }
        public Level Level { get; set; }
        public Guid LevelId { get; set; }
        public List<Question> Questions { get; set; }
        public List<AddQuestionTask> AddQuestions { get; set; }


    }
}
