using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Entities
{
    public class Level : BaseEntity
    {
        public string LevelName { get; set; }
        public List<ExamType> ExamTypes { get; set; }
        public List<LevelDetail> LevelDetails { get; set; }
    }
}
