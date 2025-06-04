using DocumentFormat.OpenXml.Office2010.Excel;
using KEB.Application.DTOs.Common;
using KEB.Application.Services;
using KEB.WebApp.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace KEB.WebApp.Controllers
{
   
    public class CommonwebController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/Common";

        public CommonwebController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDTO);
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiUrl}/login", loginDTO);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse1<string>>();

                    if (apiResponse.IsSuccess)
                    {
                        var token = apiResponse.Result;

                        // Parse token lấy claims
                        var handler = new JwtSecurityTokenHandler();
                        var jwt = handler.ReadJwtToken(token);
                        var claims = jwt.Claims.ToList();

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        // Đăng nhập sử dụng Cookie Authentication
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            principal,
                            new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                            });

                        // Optionally lưu token nếu cần
                        Response.Cookies.Append("token", token, new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTime.UtcNow.AddHours(2),
                            Secure = true,
                            SameSite = SameSiteMode.Lax
                        });

                        return RedirectToAction("Index", "Statistics");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex);
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra");
            }

            return View(loginDTO);
        }
        [HttpPost]
        public IActionResult Logout()
        {
            // Clear hết session
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Commonweb");
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.Sid).Value);
            var model = new ChangePassword
            {
                userId = userId
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Gọi API đổi mật khẩu
            var response = await _httpClient.PostAsJsonAsync($"{ApiUrl}/change-password", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Đổi mật khẩu thành công!";
                return RedirectToAction("Details", "User");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Đổi mật khẩu thất bại: {errorMessage}");
                return View(model);
            }
        }
        public IActionResult ResetPass()
        {
            return View();
        }
    }


}
