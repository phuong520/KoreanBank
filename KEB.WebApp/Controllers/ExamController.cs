using Azure;
using KEB.Application.DTOs.ExamDTO;
using KEB.Application.DTOs.ExamTypeDTO;
using KEB.Application.DTOs.LevelDTO;
using KEB.Application.DTOs.QuestionDTO;
using KEB.Application.DTOs.ReferenceDTO;
using KEB.Application.DTOs.UserDTO;
using KEB.Application.Services;
using KEB.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Policy;

namespace KEB.WebApp.Controllers
{
    public class ExamController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/Exams";
        private const string BaseUrl = "https://localhost:7101/api";

        public ExamController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        [HttpGet]
        public async Task<IActionResult> Index(GetExamAsTaskRequest request)
        {
            var token = HttpContext.Request.Cookies["token"];
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            var userId1 = Guid.Empty;
            if (jsonToken != null)
            {
                var sidClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid");
                if (sidClaim != null && Guid.TryParse(sidClaim.Value, out var parsedGuid))
                {
                    userId1 = parsedGuid;
                }
            }
            request.UserId = userId1;
            var queryParams = $"?userId={userId1}&levelId={request.LevelId}&occured={request.Occured}&host={request.Host}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<APIResponse<ExamAsTaskDisplayDTO>>($"{BaseUrl}/Tasks/view-exam-as-task{queryParams}");
                if (response == null || response.Result == null)
                {
                    TempData["Message"] = "Không có dữ liệu.";
                    return View(new List<ExamAsTaskDisplayDTO>());
                }
               
                return View(response.Result);
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Lỗi: {ex.Message}";
                return View(new List<ExamGeneralDisplayDTO>());
            }
        }

        public async Task<IActionResult> Create()
        {
            var examType = await _httpClient.GetFromJsonAsync<APIResponse<ExamTypeGeneralDisplayDTO>>($"{BaseUrl}/ExamTypes/get-all-exam-types");
            ViewBag.ExamTypes = new SelectList(examType.Result, "ExamTypeId", "ExamTypeName");
            var response = await _httpClient.GetFromJsonAsync<APIResponse<UserDisplayDTO>>($"{BaseUrl}/Users/get-all-users");
            List<UserDisplayDTO> hosts = new List<UserDisplayDTO>();
            if (response != null && response.IsSuccess)
            {
                 hosts = response.Result.Where(user => user.RoleName == "Quản lý").ToList();
            }
            ViewBag.Hosts = new SelectList(hosts, "UserId", "UserName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddExamRequest request)
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
            request.RequestedUserId = userId;
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var response = await _httpClient.PostAsJsonAsync($"{ApiUrl}/add-exam", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<APIResponse<Exam>>();
                if (result != null && result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Tạo chủ đề thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }

            ModelState.AddModelError("", "Không thể tạo chủ đề. Vui lòng thử lại.");
            return View(request);
        }

    }
}
