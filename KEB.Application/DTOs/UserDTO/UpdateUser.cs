using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.UserDTO
{
    public class UpdateUser
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public bool Gender { get; set; }
        public IFormFile AvatarImage { get; set; }
        public DateOnly DateOfBirth { get; set; }

    }
}
