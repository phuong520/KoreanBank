using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KEB.Application.DTOs.UserDTO
{
    public class ChangeAvatar
    {
        public Guid Id { get; set; }
        public IFormFile AvatarImg { get; set; }
    }
}
