using KEB.Application.DTOs.TopicDTO;
using KEB.Application.Services;
using KEB.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KEB.WebApp.Controllers
{
    public class TopicsController : Controller
    {

        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/Topics";

        public TopicsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            var result = await _httpClient.GetFromJsonAsync<APIResponse<TopicDisplayDto>>(ApiUrl);

            if (result == null || !result.IsSuccess)
            {
                return View(new List<TopicDisplayDto>());
            }

            // Trả về view với danh sách các chủ đề
            return View(result.Result);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddTopicDto topicCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return View(topicCreateDto);
            }

            var response = await _httpClient.PostAsJsonAsync($"{ApiUrl}/add-new-topic", topicCreateDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<APIResponse<TopicDisplayDto>>();
                if (result != null && result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Tạo chủ đề thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }

            ModelState.AddModelError("", "Không thể tạo chủ đề. Vui lòng thử lại.");
            return View(topicCreateDto);
        }
    }
}
