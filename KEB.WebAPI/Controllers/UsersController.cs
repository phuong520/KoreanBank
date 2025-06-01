using KEB.Application.DTOs.UserDTO;
using KEB.Application.Services;
using KEB.Domain.ValueObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KEB.WebAPI.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;
        public UsersController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }
        [HttpGet]
        [Route("get-all-users")]

        //[Authorize(Roles = "Quản trị viên")]
        public async Task<IActionResult> GetUsers(
            string keyWord = "",
            bool? isActive = null,
            bool sortAscending = true,
            int page = 1,
            int size = 10)
        {
            GetUser getUsersDTO = new()
            {
                Keyword = keyWord,
                IsActive = isActive,
                SortAscending = sortAscending,
                PaginationRequest = new() { Page = page, Size = size }
            };
            var response = await _unitOfService.UserService.GetUsers(getUsersDTO);
            if (!response.IsSuccess)
                return StatusCode((int)response.StatusCode, response);
            return Ok(response);
        }
        [HttpGet]
        [Route("get-user-by-id")]
        //[Authorize]
        public async Task<IActionResult> GetById()
        {

            var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value); 
            var userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (userId == Guid.Empty)
                return BadRequest("userId không hợp lệ.");
            var response = await _unitOfService.UserService.GetUserById(userId);
            if (!response.IsSuccess)
                return StatusCode((int)response.StatusCode, response);
            return Ok(response);
        }
        [HttpPost]
        [Route("add-user")]
        //[Authorize(Roles = "Quản trị viên")]
        public async Task<IActionResult> AddUser( UserCreateDTO userCreateDTO)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var response = new APIResponse<UserDisplayDTO>();

            response = await _unitOfService.UserService.AddUser(userCreateDTO, ipAddress ?? "");
              
            return Ok(response);
        }

        [HttpPut]
        [Route("update-profile")]
        //[Authorize]
        public async Task<IActionResult> UpdateProfile(UpdateUser userUpdateDTO)
        {
            var response = await _unitOfService.UserService
                        .UpdateUserProfile(userUpdateDTO, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");

            return Ok(response);
        }
        [HttpPut]
        [Route("change-user-avatar")]
        //[Authorize]
        public async Task<IActionResult> ChangeUserAvatar(ChangeAvatar request)
        {
            var response = await _unitOfService.UserService.ChangeUserAvatar(request, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");

            return Ok(response);
        }

        [HttpPut]
        [Route("change-role")]
        //[Authorize(Roles = "Quản trị viên")]
        public async Task<IActionResult> ChangeRole(ChangeRole request)
        {
            var response = await _unitOfService.UserService
                        .ChangeUserRole(request, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");

            return Ok(response);
        }
        [HttpPut]
        [Route("change-status")]
        //[Authorize(Roles = "Quản trị viên")]
        public async Task<IActionResult> ChangeActiveStatus(ChangeActiveStatus request)
        {
            var response = await _unitOfService.UserService
                        .ChangeActiveStatus(request, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");

            return Ok(response);
        }


    }
}
