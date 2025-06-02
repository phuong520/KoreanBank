using Azure;
using DocumentFormat.OpenXml.InkML;
using KEB.Application.DTOs.UserDTO;
using KEB.Application.Services;
using KEB.Application.Services.Implementations;
using KEB.Application.Services.Interfaces;
using KEB.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
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

        //[Authorize(Roles = "Quản trị viên")]
        public async Task<IActionResult> Index(int page = 1, int size = 10)
        {
            var url = $"{ApiUrl}/get-all-users?page={page}&size={size}";

            var result = await _httpClient.GetFromJsonAsync<APIResponse<UserDisplayDTO>>(url);

            if (result == null || !result.IsSuccess)
            {
                return View(new List<UserDisplayDTO>());
            }
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / size);

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
        public async Task<IActionResult> Create(UserCreateDTO userCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(userCreateDTO);
            }

            var token = HttpContext.Request.Cookies["token"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            var userId = Guid.Empty;
            if (jsonToken != null)
            {
                var sidClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid");
                if (sidClaim != null && Guid.TryParse(sidClaim.Value, out var parsedGuid))
                {
                    userId = parsedGuid;
                }
            }

            userCreateDTO.CreatedBy = userId;

            if (userCreateDTO.RoleId == Guid.Empty)
            {
                ModelState.AddModelError("RoleId", "Vui lòng chọn vai trò hợp lệ.");
                return View(userCreateDTO);
            }

            try
            {
                var url = $"{ApiUrl}/add-user";

                using var formData = new MultipartFormDataContent();

                // Thêm dữ liệu text (chuyển tất cả sang string trước khi thêm)
                formData.Add(new StringContent(userCreateDTO.Email ?? ""), "Email");
                formData.Add(new StringContent(userCreateDTO.FullName ?? ""), "FullName");
                formData.Add(new StringContent(userCreateDTO.DateOfBirth.ToString("yyyy-MM-dd") ?? ""), "DateOfBirth");
                formData.Add(new StringContent(userCreateDTO.Gender.ToString()), "Gender");
                formData.Add(new StringContent(userCreateDTO.RoleId.ToString()), "RoleId");
                formData.Add(new StringContent(userCreateDTO.CreatedBy.ToString()), "CreatedBy");

                // Thêm file ảnh nếu có
                if (userCreateDTO.ImageFile != null && userCreateDTO.ImageFile.Length > 0)
                {
                    var streamContent = new StreamContent(userCreateDTO.ImageFile.OpenReadStream());
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(userCreateDTO.ImageFile.ContentType);

                    formData.Add(streamContent, "ImageFile", userCreateDTO.ImageFile.FileName);
                }

                // Gửi POST multipart lên API
                var response = await _httpClient.PostAsync(url, formData);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Không thể tạo người dùng mới: {errorMessage}");
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Đã xảy ra lỗi khi tạo người dùng: {ex.Message}";
                return View("Error");
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var token = Request.Cookies["token"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"{ApiUrl}/get-user-by-id";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<APIResponse1<UserDisplayDTO>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (apiResponse?.IsSuccess == true)
                {
                    var user = apiResponse.Result;
                    var updateUser = new UpdateUser
                    {
                        //AvatarImage = user.AvatarUrl,
                        UserId = user.UserId,
                        FullName = user.FullName,
                        Gender = user.Gender,
                        DateOfBirth = user.DateOfBirth,


                    };

                    // Truyền thông tin avatar hiện tại qua ViewBag
                    ViewBag.CurrentAvatarUrl = user.AvatarUrl;
                    ViewBag.Email = user.Email;
                    ViewBag.PhoneNumber = user.PhoneNumber;

                    return View(updateUser);
                }
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound("Không tìm thấy người dùng");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateUser updateUser)
        {
            if (id != updateUser.UserId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(updateUser);
            }

            try
            {
                // Prepare multipart form data for file upload
                using var formData = new MultipartFormDataContent();

                // Add form fields
                formData.Add(new StringContent(updateUser.UserId.ToString()), "UserId");
                formData.Add(new StringContent(updateUser.FullName ?? ""), "FullName");
                formData.Add(new StringContent(updateUser.Gender.ToString()), "Gender");
                formData.Add(new StringContent(updateUser.DateOfBirth.ToString("yyyy-MM-dd")), "DateOfBirth");

                // Add file if exists
                if (updateUser.AvatarImage != null && updateUser.AvatarImage.Length > 0)
                {
                    var fileContent = new StreamContent(updateUser.AvatarImage.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(updateUser.AvatarImage.ContentType);
                    formData.Add(fileContent, "AvatarImage", updateUser.AvatarImage.FileName);
                }

                var response = await _httpClient.PutAsync($"{ApiUrl}/update-profile", formData);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<APIResponse<UserDisplayDTO>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse?.IsSuccess == true)
                    {
                        TempData["SuccessMessage"] = apiResponse.Message ?? "Cập nhật thông tin thành công!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", apiResponse?.Message ?? "Có lỗi xảy ra khi cập nhật");
                    }
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<APIResponse<object>>(errorContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    ModelState.AddModelError("", errorResponse?.Message ?? "Dữ liệu không hợp lệ");
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound("Không tìm thấy người dùng");
                }
                else
                {
                    ModelState.AddModelError("", "Lỗi server khi cập nhật thông tin");
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Lỗi kết nối: " + ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            return View(updateUser);
        }

    }
}
