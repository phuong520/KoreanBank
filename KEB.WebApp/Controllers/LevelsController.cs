using KEB.Application.DTOs.LevelDTO;
using KEB.Application.DTOs.TopicDTO;
using KEB.Application.DTOs.UserDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebApp.Controllers
{
    public class LevelsController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/Levels";

        public LevelsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<IActionResult> Index()
        {
            var url = $"{ApiUrl}/get-all-levels";

            var result = await _httpClient.GetFromJsonAsync<APIResponse<LevelDisplayBriefDTO>>(url);

            if (result == null || !result.IsSuccess)
            {
                return View(new List<LevelDisplayBriefDTO>());
            }

            return View(result.Result);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Gọi API để lấy danh sách chủ đề
            var response = await _httpClient.GetFromJsonAsync<APIResponse<List<TopicDisplayDto>>>($"{ApiUrl}/topics");

           /// ViewBag.Topics = response?.Result ?? new List<TopicDisplayDto>();

            return View(); // Trả về View chứa form tạo mới
        }



    }
}
