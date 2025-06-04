using KEB.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.UserDTO
{
    public class UserDisplayDTO
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public bool Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string PhoneNumber { get; set; }
       // public bool PasswordNeedChange { get; set; }
    }
}
