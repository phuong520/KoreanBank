using KEB.Application.DTOs.ReferenceDTO;
using KEB.Application.DTOs.TopicDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebApp.Controllers
{
    public class ReferencesController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/References";

        public ReferencesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task< IActionResult> Index()
        {
            var url = $"{ApiUrl}/get-all-references";
            var result = await _httpClient.GetFromJsonAsync<APIResponse<ReferenceDisplayDto>>(url);

            // Kiểm tra nếu không có kết quả hoặc có lỗi
            if (result == null || !result.IsSuccess)
            {
                return View(new List<ReferenceDisplayDto>());
            }

            // Trả về view với danh sách các chủ đề
            return View(result.Result);
        }
    }
}
