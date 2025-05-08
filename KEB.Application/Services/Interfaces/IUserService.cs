using KEB.Application.DTOs.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<APIResponse<UserDisplayDTO>> GetUsers(GetUser request);
        Task<APIResponse1<UserDisplayDTO>> GetUserById(Guid userId);
        Task<APIResponse<UserDisplayDTO>> AddUser(UserCreateDTO userCreateDTO, string ipAddress);
        Task<APIResponse<UserDisplayDTO>> UpdateUserProfile(UpdateUser request, string ipAddress);
        Task<APIResponse<UserDisplayDTO>> ChangeUserAvatar(ChangeAvatar request, string ipAddress);
        Task<APIResponse<UserDisplayDTO>> ChangeUserRole(ChangeRole request, string ipAddress);
        Task<APIResponse<UserDisplayDTO>> ChangeActiveStatus(ChangeActiveStatus request, string ipAddress);

        Task<APIResponse<UserDisplayDTO>> ChangePassCustomize(Guid userId, string input);

    }
}
