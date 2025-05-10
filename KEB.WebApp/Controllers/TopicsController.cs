using KEB.Application.DTOs.LevelDTO;
using KEB.Application.DTOs.TopicDTO;
using KEB.Application.Services;
using KEB.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace KEB.WebApp.Controllers
{
    public class TopicsController : Controller
    {

        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/Topics";
        private const string BaseApiUrl = "https://localhost:7101/api";

        public TopicsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            var result = await _httpClient.GetFromJsonAsync<APIResponse<TopicDisplayDto>>(ApiUrl);
            var levels = await _httpClient.GetFromJsonAsync<APIResponse<LevelDisplayBriefDTO>>($"{BaseApiUrl}/Levels/get-all-levels");
            ViewBag.Levels = new SelectList(levels.Result, "LevelId", "LevelName");
            if (result == null || !result.IsSuccess)
            {
                return View(new List<TopicDisplayDto>());
            }

            // Trả về view với danh sách các chủ đề
            return View(result.Result);
        }
        [HttpPost]
        public async Task<IActionResult> Create(AddTopicDto topicCreateDto)
      {
            if (!ModelState.IsValid)
            {
                return View(topicCreateDto);
            }
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
            topicCreateDto.CreatedBy = userId;
            topicCreateDto.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            topicCreateDto.Description = "";
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
