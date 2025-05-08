using AutoMapper;
using KEB.Application.DTOs.RoleDTO;
using KEB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Mappings
{
    public class RoleMapper :Profile
    {
        public RoleMapper()
        {
            CreateMap<Role, RoleDisplayDto>().ReverseMap();
        }
    }
}
