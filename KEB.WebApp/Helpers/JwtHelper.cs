using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace KEB.WebApp.Helpers
{
    public static class JwtHelper
    {
        public static (string Username, string Role) DecodeJwt(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(jwtToken);

            var username = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "Username")?.Value ?? "";
            var role = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "Role")?.Value ?? "";

            return (username, role);
        }
    }
}
