using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Domain.Entities
{
    public class SystemAccessLog: BaseEntity
    {
        public string IpAddress { get; set; }
        public DateTime AccessTime { get; set; }
        public string TargetObject { get; set; }
        public string ActionName { get; set; }
        public string Details { get; set; }
        public bool IsSuccess { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public List<Question> Questions { get; set; }
    }
}
