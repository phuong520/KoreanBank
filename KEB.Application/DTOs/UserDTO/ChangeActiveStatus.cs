using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.UserDTO
{
    public class ChangeActiveStatus
    {
        public Guid TargertUserId { get; set; }
        public Guid UpdatedBy {  get; set; }
    }
}
