using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Entities
{
    public class Topic : BaseEntity
    {
        public string TopicName { get; set; }
        public string? Description { get; set; }
        public List<LevelDetail> LevelDetails { get; set; }
        public List<ConstraintDetail> ConstraintDetails { get; set; }
    }
}
