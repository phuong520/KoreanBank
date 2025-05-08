using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.UserDTO
{
    public class ChangeRole
    {
        public Guid TargetUserId { get; set; }
        public Guid RoleId { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
