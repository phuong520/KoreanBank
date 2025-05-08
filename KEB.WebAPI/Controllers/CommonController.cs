using KEB.Application.DTOs.Common;
using KEB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;

        public CommonController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            try
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true, 
                    Secure = false,   
                    SameSite = SameSiteMode.Lax,  
                    Expires = DateTime.UtcNow.AddHours(2) 
                };

                var response = await _unitOfService.CommonService.LoginUserAsync(loginDTO);
                Response.Cookies.Append("token", response.Result, cookieOptions);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePassword changePassDTO)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var response = await _unitOfService.CommonService.ChangePasswordAsync(changePassDTO, ipAddress);

            return Ok(response);
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassDTO)
        {
            var response = await _unitOfService.CommonService.ResetPasswordAsync(resetPassDTO);

            return Ok(response);
        }

    }
}
