using DocumentFormat.OpenXml.Office2010.Excel;
using KEB.Application.DTOs.Common;
using KEB.Application.Services;
using KEB.WebApp.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebApp.Controllers
{
    public class CommonController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/Common";

        public CommonController(IHttpClientFactory httpClientFactory)
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
                        // Gán lại cookie trong MVC
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = false,
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTime.UtcNow.AddHours(2)
                        };

                        Response.Cookies.Append("token", apiResponse.Result, cookieOptions);

                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi "+ ex);
                return View(loginDTO);
            }
            return BadRequest("Unknown error occurred");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Clear hết session
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Common");
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            var userIdClaim = User.FindFirst("UserId");
            var model = new ChangePassword
            {
                userId = Guid.Parse(userIdClaim.Value)
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
                return RedirectToAction("Profile", "Account");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Đổi mật khẩu thất bại: {errorMessage}");
                return View(model);
            }
        }
    }


}
