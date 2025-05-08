using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ImportQuestionTaskDTO;
using KEB.Application.DTOs.LevelDTO;
using KEB.Application.DTOs.LevelTopicDetailDTO;
using KEB.Application.DTOs.QuestionTypeDTO;
using KEB.Application.DTOs.UserDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace KEB.WebApp.Controllers
{
    //[Authorize]
    public class TaskController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/Tasks";
        private const string BaseApiUrl = "https://localhost:7101/api";

        public TaskController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<APIResponse<TaskGeneralDisplayDTO>>($"{ApiUrl}/view-import-question-tasks");
                if (!result.IsSuccess)
                {
                    TempData["Error"] = "Không thể tải danh sách nhiệm vụ";
                    return View(new List<TaskGeneralDisplayDTO>());
                }
                return View(result.Result);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                return View(new List<TaskGeneralDisplayDTO>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var token = HttpContext.Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Common", new { returnUrl = Url.Action("Details", "Task", new { id }) });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{ApiUrl}/view-task-by-id?id={id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không tìm thấy nhiệm vụ";
                    return RedirectToAction(nameof(Index));
                }

                var result = await response.Content.ReadFromJsonAsync<APIResponse<List<TaskFullDisplayDTO>>>();
                if (result?.IsSuccess != true || result.Result == null || !result.Result.Any())
                {
                    TempData["Error"] = "Không tìm thấy thông tin chi tiết nhiệm vụ";
                    return RedirectToAction(nameof(Index));
                }

                return View(result.Result.First());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra khi tải chi tiết nhiệm vụ: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<APIResponse<DetailDisplayDTO>> GetTopic(Guid levelId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseApiUrl}/LevelDetails/get-detail-by-level-id/{levelId}");

                if (!response.IsSuccessStatusCode)
                {
                    return new APIResponse<DetailDisplayDTO>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi gọi API nội bộ: {(int)response.StatusCode} {response.ReasonPhrase}"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<APIResponse<DetailDisplayDTO>>();

                if (result == null)
                {
                    return new APIResponse<DetailDisplayDTO>
                    {
                        IsSuccess = false,
                        Message = "Không đọc được nội dung phản hồi từ API nội bộ"
                    };
                }

                return result;
            }
            catch (Exception ex)
            {
                return new APIResponse<DetailDisplayDTO>
                {
                    IsSuccess = false,
                    Message = $"Lỗi ngoại lệ: {ex.Message}"
                };
            }

        }
        public async Task<IActionResult> Create()
        {
            var token = HttpContext.Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Common", new { returnUrl = Url.Action("Create", "Question") });
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var lecturers = await _httpClient.GetFromJsonAsync<APIResponse<UserDisplayDTO>>($"{BaseApiUrl}/Users/get-all-users");
            var levels = await _httpClient.GetFromJsonAsync<APIResponse<LevelDisplayBriefDTO>>($"{BaseApiUrl}/Levels/get-all-levels");
            var types = await _httpClient.GetFromJsonAsync<APIResponse<QuestionTypeDisplayDto>>($"{BaseApiUrl}/QuestionTypes/get-all-questiontypes");
            if(!lecturers.IsSuccess || !levels.IsSuccess || !types.IsSuccess)
            {
                ModelState.AddModelError("", "Không thể tải dữ liệu từ API.");
                return View(new AssignTaskRequest());
            }
            ViewBag.Levels = new SelectList(levels.Result, "LevelId", "LevelName");
            ViewBag.QuestionTypes = new SelectList(types.Result, "QuestionTypeId", "QuestionTypeName");
            ViewBag.Users = new SelectList(lecturers.Result, "UserId", "UserName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssignTaskRequest request)
        {
            try
            {
                var token = HttpContext.Request.Cookies["token"];
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
                request.RequestedUserId = userId;
                request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                if (!ModelState.IsValid)
                {
                    foreach (var modelState in ModelState)
                    {
                        var key = modelState.Key;
                        var errors = modelState.Value.Errors;

                        foreach (var error in errors)
                        {
                            Console.WriteLine($"❌ Lỗi ở '{key}': {error.ErrorMessage}");
                            // hoặc dùng logger:
                            // _logger.LogWarning($"Lỗi ở '{key}': {error.ErrorMessage}");
                        }
                    }
                    return View(request);
                }

                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Common", new { returnUrl = Url.Action("Create", "Task") });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{ApiUrl}/assign-import-question-task", content);
                var resultString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<APIResponse<TaskGeneralDisplayDTO>>(resultString);

                if (response.IsSuccessStatusCode && result?.IsSuccess == true)
                {
                    TempData["Success"] = "Tạo nhiệm vụ thành công!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = $"Tạo nhiệm vụ thất bại: {result?.Message ?? "Lỗi không xác định"}";
                return View(request);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
                return View(request);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var token = HttpContext.Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Common", new { returnUrl = Url.Action("Index", "Task") });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var deleteRequest = new Delete {TargetObjectId = id };
                var jsonContent = JsonSerializer.Serialize(deleteRequest);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.DeleteAsync($"{ApiUrl}/delete-import-question-task");
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Xóa nhiệm vụ thành công!";
                }
                else
                {
                    TempData["Error"] = "Xóa nhiệm vụ thất bại";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeDeadline(ChangeTaskDeadlineRequest request)
        {
            try
            {
                var token = HttpContext.Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Common", new { returnUrl = Url.Action("Details", "Task", new { id = request.TargetTaskId }) });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{ApiUrl}/change-task-deadline", content);
                var resultString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<APIResponse<TaskGeneralDisplayDTO>>(resultString);

                if (response.IsSuccessStatusCode && result?.IsSuccess == true)
                {
                    TempData["Success"] = "Cập nhật hạn hoàn thành thành công!";
                }
                else
                {
                    TempData["Error"] = $"Cập nhật hạn hoàn thành thất bại: {result?.Message ?? "Lỗi không xác định"}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id = request.TargetTaskId });
        }
    }
} 