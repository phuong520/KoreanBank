using KEB.Application.DTOs.ImportQuestionTaskDTO;
using KEB.Application.DTOs.SystemAccessLogDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebApp.Controllers
{
    public class AccessLogController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/AccessLogs";
        private const string BaseApiUrl = "https://localhost:7101/api";

        public AccessLogController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        //[Authorize(Roles = "Quản trị viên")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var request = new ViewAccessLog(); // Gửi request trống hoặc có filter tùy ý

                var response = await _httpClient.PostAsJsonAsync($"{ApiUrl}", request);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không thể tải danh sách hoạt động";
                    return View(new List<AccessLogDisplayDto>());
                }

                var result = await response.Content.ReadFromJsonAsync<APIResponse<AccessLogDisplayDto>>();
                if (result.IsSuccess)
                {
                    ViewBag.Pagination = result.Pagination;
                    ViewBag.TotalCount = result.Result.Count;
                    return View(result.Result);
                }
                return View(result.Result);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                return View(new List<AccessLogDisplayDto>());
            }
        }
    }
}
