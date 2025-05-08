using KEB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IUnitOfService _unitOfService;

        public RolesController(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }
        [HttpGet]
        [Route("get-all-roles")]
        //[Authorize]
        public async Task<IActionResult> GetRoles()
        {
            var response = await _unitOfService.RoleService.GetRoles();
            return Ok(response);
        }
    }
}
