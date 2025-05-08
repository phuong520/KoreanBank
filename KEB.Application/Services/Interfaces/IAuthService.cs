using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface IAuthService
    {

        Task<APIResponse1<string>> LoginUserAsync(LoginDTO loginDTO);
        Task<APIResponse<UserDisplayDTO>> ChangePasswordAsync(ChangePassword userChangePasswordDTO, string ipAddress);
        Task<APIResponse<UserDisplayDTO>> ResetPasswordAsync(ResetPassword userResetPasswordDTO);

    }
}
