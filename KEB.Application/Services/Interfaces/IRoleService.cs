using KEB.Application.DTOs.RoleDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface IRoleService
    {
        Task<APIResponse<RoleDisplayDto>> GetRoles();
    }
}
