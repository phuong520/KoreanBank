using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.LevelDTO
{
    public class AddLevelRequest
    {
        public Guid RequestedUserId { get; set; }
        public string LevelName { get; set; }
        public List<Guid>? Topics { get; set; }

    }
}
