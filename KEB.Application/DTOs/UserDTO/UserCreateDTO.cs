using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.UserDTO
{
    public class UserCreateDTO
    {

        [Required]
        public string Email { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        public bool Gender { get; set; }

        public IFormFile? ImageFile { get; set; }

        public DateTime DateOfBirth { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid RoleId { get; set; }
    }
}
