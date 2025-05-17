
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.NotificationDTO
{
    public class NotificationDisplayDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedTime { get; set; }

    }
}
