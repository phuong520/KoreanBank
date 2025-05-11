using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Entities
{
    public class Exam :BaseEntity
    {
        public string ExamName { get; set; }
        public DateTime TakePlaceTime { get; set; }
        public bool IsSuspended { get; set; }
        public ExamType ExamType { get; set; }
        public Guid ExamTypeId { get; set; }
        public List<Paper> Papers { get; set; }
        public User Host { get; set; }
        public Guid HostId { get; set; }
        public User Reviewer { get; set; }
        public Guid ReviewerId { get; set; }
    }
}
