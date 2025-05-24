using KEB.Application.DTOs.ReferenceDTO;
using KEB.Application.DTOs.TopicDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

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
            ViewBag.CurrentUserId = userId;
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

        public async Task<IActionResult> Create()
        {
            return View();
        }

       [HttpPost]
        public async Task<IActionResult> Create(AddReferenceDto request)
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
            request.CreatedBy = userId;
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var response = await _httpClient.PostAsJsonAsync($"{ApiUrl}/add-ref", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<APIResponse<ReferenceDisplayDto>>();
                if (result != null && result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Tạo chủ đề thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }

            ModelState.AddModelError("", "Không thể tạo chủ đề. Vui lòng thử lại.");
            return View(request);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _httpClient.GetFromJsonAsync<APIResponse<ReferenceDisplayDto>>($"{ApiUrl}/get-reference-details-id-{id}");
            if (response == null || !response.IsSuccess || response.Result == null || !response.Result.Any())
            {
                TempData["ErrorMessage"] = response?.Message ?? "Không tìm thấy tài liệu.";
                return RedirectToAction(nameof(Index));
            }

            var reference = response.Result.First();
            var model = new UpdateReference
            {
                TargetObjectId = reference.Id,
                ReferenceName = reference.ReferenceName,
                PublishedYear = reference.PublishedYear,
                Description = reference.Description,
                ReferenceLink = reference.ReferenceLink,
                ReferenceAuthor = reference.ReferenceAuthor
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateReference request)
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

            request.CreatedBy = userId;
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var response = await _httpClient.PutAsJsonAsync($"{ApiUrl}/update-reference", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<APIResponse<ReferenceDisplayDto>>();
                if (result != null && result.IsSuccess)
                {
                    TempData["SuccessMessage"] = result.Message ?? "Cập nhật tài liệu thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<APIResponse<ReferenceDisplayDto>>();
            ModelState.AddModelError("", errorResponse?.Message ?? "Không thể cập nhật tài liệu. Vui lòng thử lại.");
            return View(request);
        }
    }
}
