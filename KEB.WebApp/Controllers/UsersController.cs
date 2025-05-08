using DocumentFormat.OpenXml.InkML;
using KEB.Application.DTOs.UserDTO;
using KEB.Application.Services;
using KEB.Application.Services.Implementations;
using KEB.Application.Services.Interfaces;
using KEB.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace KEB.WebApp.Controllers
{

    public class UsersController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/Users";

        public UsersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            var url = $"{ApiUrl}/get-all-users?page=1&size=10";

            var result = await _httpClient.GetFromJsonAsync<APIResponse<UserDisplayDTO>>(url);

            if (result == null || !result.IsSuccess)
            {
                return View(new List<UserDisplayDTO>());
            }

            return View(result.Result);
        }
        public async Task<IActionResult> Details()
        {
            var token = Request.Cookies["token"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Gọi API để lấy thông tin người dùng
            var url = $"{ApiUrl}/get-user-by-id";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Không tìm thấy người dùng.";
                return View("Error");
            }

            var content = await response.Content.ReadFromJsonAsync<APIResponse1<UserDisplayDTO>>();

            return View(content.Result); 
        }

        // GET: User/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateDTO userCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(userCreateDTO);
            }
            if (userCreateDTO == null || userCreateDTO.RoleId == Guid.Empty)
            {
                ModelState.AddModelError("RoleId", "Vui lòng chọn vai trò hợp lệ.");
                return View(userCreateDTO);
            }
            try
            {
                var url = $"{ApiUrl}/add-user";

                // Gửi yêu cầu POST lên API
                var response = await _httpClient.PostAsJsonAsync(url, userCreateDTO);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Nếu không thành công
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = $"Không thể tạo người dùng mới: {errorMessage}";
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có exception
                ViewBag.Error = $"Đã xảy ra lỗi khi tạo người dùng: {ex.Message}";
                return View("Error");
            }
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _httpClient.GetAsync($"{ApiUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<User>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{ApiUrl}/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error updating user");
            }
            return View(user);
        }
        
    }
}
