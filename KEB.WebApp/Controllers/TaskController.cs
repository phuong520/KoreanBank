using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ImportQuestionTaskDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace KEB.WebApp.Controllers
{
    //[Authorize]
    public class TaskController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/Tasks";

        public TaskController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            var result =  await _httpClient.GetFromJsonAsync<APIResponse<TaskGeneralDisplayDTO>>($"{ApiUrl}/view-import-question-tasks");
            return View(result.Result);
        }

       


        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiUrl}/view-task-by-id?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<APIResponse<List<TaskFullDisplayDTO>>>();
                    if (result?.IsSuccess == true && result.Result.Any())
                    {
                        return View(result.Result.First());
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải chi tiết nhiệm vụ: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        public IActionResult Create()
        {
            return View();
        }

        // Xử lý tạo task mới và gọi API
        [HttpPost]
        public async Task<IActionResult> Create(ImportQuestionTaskDetail taskDetail)
        {
            if (ModelState.IsValid)
            {
                // Chuẩn bị dữ liệu gửi lên API
                var jsonContent = JsonSerializer.Serialize(taskDetail);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                // Gửi POST request đến API
                var response = await _httpClient.PostAsync(ApiUrl, content);

                // Kiểm tra phản hồi từ API
                if (response.IsSuccessStatusCode)
                {
                    // Xử lý thành công
                    ViewBag.Message = "Task đã được tạo thành công!";
                }
                else
                {
                    // Xử lý lỗi
                    ViewBag.Message = "Đã có lỗi xảy ra khi tạo task.";
                }
            }
            return View();
        }

    }
} 