using AutoMapper;
using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.UserDTO;
using KEB.Application.Services.Interfaces;
using KEB.Domain.Entities;
using KEB.Domain.ValueObject;
using KEB.Infrastructure.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static KEB.Domain.ValueObject.LogicString;

namespace KEB.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork ;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;

        }
        public async Task<APIResponse<UserDisplayDTO>> ChangePasswordAsync(ChangePassword userChangePasswordDTO, string ipAddress)
        {
            var response = new APIResponse<UserDisplayDTO>();

            var user = await _unitOfWork.Users.GetByIdAsync(userChangePasswordDTO.userId);
            if (user == null || !user.IsActive)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.Message = AppMessages.LOCKED_ACCOUNT;
                return response;
            }
            // Hash mật khẩu cũ người dùng nhập vào
            string hashedOldPassword = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(userChangePasswordDTO.OldPassword))).Replace("-", "").ToLower();
           // string hashedOldPassword = HashPasswordSHA256(userChangePasswordDTO.OldPassword);
            // Kiểm tra mật khẩu cũ có đúng không
            if (user.Password != hashedOldPassword)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Message = "Mật khẩu cũ không đúng.";
                return response;
            }
            // Hash mật khẩu mới và lưu lại
            user.Password = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(userChangePasswordDTO.NewPassword))).Replace("-", "").ToLower();
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            // Ghi log
            await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog()
            {
                UserId = user.Id,
                IsSuccess = true,
                TargetObject = "UserController",
                ActionName = "ChangePassword",
                Details = $"Người dùng {user.UserName} đã đổi mật khẩu.",
                IpAddress = ipAddress
            });
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Message = "Đổi mật khẩu thành công.";
           // response.Result = _mapper.Map<UserDisplayDTO>(user);
            return response;
        }
        
        public async Task<APIResponse1<string>> LoginUserAsync(LoginDTO loginDTO)
        {
            APIResponse1<string> response = new();
            if (loginDTO.Password != null && loginDTO.Username != null)
            {
                
                var user = await _unitOfWork.Common.LoginAsync(loginDTO.Username, loginDTO.Password);
                if (user != null)
                {
                    if (user.IsActive)
                    {
                       
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim("avatar", user.Avatar.ToString()),
                            new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "Unknown"),
                            new Claim(ClaimTypes.Sid, user.Id.ToString())
                        };
                        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                        var signInCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var tokenDesc = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(claims),
                            Expires = DateTime.Now.AddMinutes(60),
                            SigningCredentials = signInCredentials,
                            Issuer = _configuration["Jwt:Issuer"],
                            Audience = _configuration["Jwt:Audience"],
                        };
                        var tempToken = tokenHandler.CreateToken(tokenDesc);

                        string tokenValue = new JwtSecurityTokenHandler().WriteToken(tempToken);
                        response.StatusCode = System.Net.HttpStatusCode.OK;
                        response.IsSuccess = true;
                        response.Result = tokenValue;
                        response.Message = AppMessages.LOGIN_SUCCESS;
                        //log
                        await _unitOfWork.AccessLogs.AddAsync(new SystemAccessLog()
                        {
                            UserId = user.Id,
                            IsSuccess = true,
                            TargetObject = AccessLogConstant.COMMON_CONTROLLER,
                            ActionName = AccessLogConstant.LOGINACTION,
                            Details = string.Format(AccessLogConstant.LOGIN_SUCCESS, user.UserName),
                            AccessTime = DateTime.UtcNow,
                            IpAddress = ""
                        });
                        return response;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.StatusCode = System.Net.HttpStatusCode.OK;
                        response.Message = AppMessages.LOCKED_ACCOUNT;
                        return response;
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Message = AppMessages.WRONG_LOGIN_INFO;
                    return response;
                }
            }
            else if (loginDTO.Password == null && loginDTO.Username != null)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Message = AppMessages.PASSWORD_REQUIRED;
                return response;
            }
            else
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Message = AppMessages.USERNAME_REQUIRED;
                return response;

            }
        }

        public Task<APIResponse<UserDisplayDTO>> ResetPasswordAsync(ResetPassword userResetPasswordDTO)
        {
            throw new NotImplementedException();
        }
    }
}
