using Azure;
using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ExamPaperDTO;
using KEB.Application.DTOs.ImportQuestionTaskDTO;
using KEB.Application.DTOs.LevelDTO;
using KEB.Application.DTOs.LevelTopicDetailDTO;
using KEB.Application.DTOs.QuestionDTO;
using KEB.Application.DTOs.QuestionTypeDTO;
using KEB.Application.DTOs.UserDTO;
using KEB.Application.Services;
using KEB.Domain.Enums;
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

        public async Task<IActionResult> Index(int page = 1, int size = 10)
        {
            try
            {

                var result = await _httpClient.GetFromJsonAsync<APIResponse<TaskGeneralDisplayDTO>>($"{ApiUrl}/view-import-question-tasks?page=1&&size=10");
                if (!result.IsSuccess)
                {
                    TempData["Error"] = "Không thể tải danh sách nhiệm vụ";
                    return View(new List<TaskGeneralDisplayDTO>());
                }
                ViewBag.Page = result.Pagination.Page;
                ViewBag.Size = result.Pagination.Size;
                ViewBag.TotalCount = result.TotalCount;
                ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / result.Pagination.Size);
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
              
                var response = await _httpClient.GetAsync($"{ApiUrl}/view-task-by-id?id={id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không tìm thấy nhiệm vụ";
                    return RedirectToAction(nameof(Index));
                }

                var result = await response.Content.ReadFromJsonAsync<APIResponse<TaskFullDisplayDTO>>();
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
                return RedirectToAction("Login", "Commonweb", new { returnUrl = Url.Action("Create", "Question") });
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
                    return RedirectToAction("Login", "Commonweb", new { returnUrl = Url.Action("Details", "Task", new { id = request.TargetTaskId }) });
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

        public async Task<IActionResult> ReviewQuestion(GetQuestionsRequest request, int page= 1, int size = 10)
        {
            try
            {
                var token = HttpContext.Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Commonweb");
                }
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.PaginationRequest = new Pagination { Page = page, Size = size };
                var response = await _httpClient.PostAsJsonAsync($"{BaseApiUrl}/Questions/get-questions", request);
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse<QuestionDisplayDto>>();

                    // Pass current status to ViewBag to maintain tab selection
                    ViewBag.CurrentStatus = request.Status;
                    ViewBag.Page = apiResponse.Pagination.Page;
                    ViewBag.Size = apiResponse.Pagination.Size;
                    ViewBag.TotalCount = apiResponse.TotalCount;
                    ViewBag.TotalPages = (int)Math.Ceiling((double)apiResponse.TotalCount / apiResponse.Pagination.Size);
                    return View(apiResponse.Result);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewQuestion(ChangeQuestionStatusRequest request)
        {
            try
            {
                var token = HttpContext.Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Commonweb", new { returnUrl = Url.Action("ReviewQuestion", "Task") });
                }
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

                // Set the user ID making the request
                request.RequestedUserId = userId;
                request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
                
                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{ApiUrl}/lead-change-question-status", content);
                var resultString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<APIResponse<ChangeStatusResultDTO>>(resultString);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Duyệt câu hỏi thành công!";
                }
                else
                {
                    // Display specific error message if available
                    TempData["Error"] = result?.Message ?? "Duyệt câu hỏi thất bại";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
            }

            // Return to the same tab view new { status = request.Requests.First().ToStatus }
            return RedirectToAction(nameof(ReviewQuestion), new { status = request.Requests?.FirstOrDefault()?.ToStatus ?? QuestionStatus.Ok });
        }

        [HttpGet]
        public async Task<IActionResult> GetQuestionDetailsForReview(Guid id)
        {
            try
            {
                var token = HttpContext.Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Commonweb");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Call the API to get question details
                var response = await _httpClient.GetAsync($"{BaseApiUrl}/Questions/get-question-has-id-{id}");

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse<QuestionDisplayDto>>();

                    if (apiResponse.IsSuccess && apiResponse.Result != null && apiResponse.Result.Count > 0)
                    {
                        var questionDetail = apiResponse.Result[0];

                        // Extract LogId and CreatedBy information
                        var logId = questionDetail.LogId;
                        var createdById = questionDetail.NotifyTo;

                        // Return as JSON for AJAX usage
                        return Json(new
                        {
                            success = true,
                            logId = logId,
                            createdBy = createdById
                        });
                    }

                    return Json(new { success = false, message = "Không tìm thấy thông tin câu hỏi" });
                }

                return Json(new { success = false, message = "Không thể lấy thông tin câu hỏi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        public async Task<IActionResult> ReviewPaper(ViewExamPapersListRequest request)
        {
            try
            {
                var token = HttpContext.Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Commonweb");
                }
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsJsonAsync($"{BaseApiUrl}/Questions/get-questions", request);
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse<PaperGeneralDisplayDTO>>();
                    return View(apiResponse.Result);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 