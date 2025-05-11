using KEB.Application.DTOs.ExamDTO;
using KEB.Application.DTOs.QuestionTypeDTO;
using KEB.Application.DTOs.ReferenceDTO;
using KEB.Application.Services;
using KEB.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Policy;

namespace KEB.WebApp.Controllers
{
    public class QuestionTypeController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/QuestionTypes";

        public QuestionTypeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<IActionResult> Index()
        {
            var result = await _httpClient.GetFromJsonAsync<APIResponse<QuestionTypeDisplayDto>>($"{ApiUrl}/get-all-questiontypes");
            return View(result.Result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(QuestionTypeCreateDto request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);

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
            request.CreatedUserId = userId;

            var response = await _httpClient.PostAsJsonAsync($"{ApiUrl}/add-questiontype", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<APIResponse<QuestionTypeDisplayDto>>();
                if (result != null && result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Tạo loại câu hỏi thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }

            ModelState.AddModelError("", "Không thể tạo loại câu hỏi. Vui lòng thử lại.");
            return View(request);
        }

    }
}
