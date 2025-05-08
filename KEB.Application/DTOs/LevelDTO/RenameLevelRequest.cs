using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.LevelDTO
{
    public class RenameLevelRequest
    {
        public Guid RequestedUserId { get; set; }
        public Guid TargetLevelId { get; set; }
        public string NewLevelName { get; set; }

    }
}
