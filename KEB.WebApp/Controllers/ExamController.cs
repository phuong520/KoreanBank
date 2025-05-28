using Azure;
using KEB.Application.DTOs.ExamDTO;
using KEB.Application.DTOs.ExamTypeDTO;
using KEB.Application.DTOs.LevelDTO;
using KEB.Application.DTOs.QuestionDTO;
using KEB.Application.DTOs.ReferenceDTO;
using KEB.Application.DTOs.UserDTO;
using KEB.Application.Services;
using KEB.Domain.Entities;
using KEB.Domain.ValueObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text.Json;

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
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                // Lấy thông tin exam cần sửa
                var examResponse = await _httpClient.GetFromJsonAsync<APIResponse<ExamComplexDisplayDTO>>($"{BaseUrl}/Exams/get-exam-by-id?id={id}");

                if (examResponse == null || !examResponse.IsSuccess || examResponse.Result == null || !examResponse.Result.Any())
                {
                    TempData["ErrorMessage"] = "Không tìm thấy bài thi cần sửa.";
                    return RedirectToAction("Index");
                }

                var exam = examResponse.Result.First();

                // Lấy danh sách ExamType cho dropdown
                var examTypeResponse = await _httpClient.GetFromJsonAsync<APIResponse<ExamTypeGeneralDisplayDTO>>($"{BaseUrl}/ExamTypes/get-all-exam-types");
                var examTypes = examTypeResponse?.Result ?? new List<ExamTypeGeneralDisplayDTO>();
                ViewBag.ExamTypes = new SelectList(examTypes, "ExamTypeId", "ExamTypeName");

                // Lấy danh sách Host (Quản lý) cho dropdown
                var userResponse = await _httpClient.GetFromJsonAsync<APIResponse<UserDisplayDTO>>($"{BaseUrl}/Users/get-all-users");
                List<UserDisplayDTO> hosts = new List<UserDisplayDTO>();
                if (userResponse != null && userResponse.IsSuccess && userResponse.Result != null)
                {
                    hosts = userResponse.Result.Where(user => user.RoleName == "Quản lý").ToList();
                }
                ViewBag.Hosts = new SelectList(hosts, "UserId", "UserName");

                // Lấy danh sách Reviewer cho dropdown
                var reviewers = userResponse?.Result?.Where(user => user.RoleName == "Reviewer" || user.RoleName == "Quản lý").ToList() ?? new List<UserDisplayDTO>();
                ViewBag.Reviewers = new SelectList(reviewers, "UserId", "UserName");
                // Tìm ID theo tên
                var currentExamTypeId = examTypes.FirstOrDefault(et => et.ExamTypeName == exam.ExamTypeName)?.ExamTypeId;
                var currentHostId = hosts.FirstOrDefault(u => u.UserName == exam.HostUserName)?.UserId;
                var currentReviewerId = reviewers.FirstOrDefault(u => u.UserName == exam.ReviewerUserName)?.UserId;

                // Truyền model exam vào view
                var editModel = new EditExamRequest
                {
                     TargetObjectId = exam.ExamId,
                    NewExamName = exam.Examname,
                    NewExamTypeId = currentExamTypeId,
                    NewTakePlaceTime = exam.TakePlaceTime,
                    NewHostId = currentHostId,
                    NewReviewerId = currentReviewerId,
                    IpAddress = ""
                };

                return View(editModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin bài thi.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditExamRequest request)
        {
            if (!ModelState.IsValid)
            {
                // Tải lại danh sách ExamTypes và Users
                var examTypesResponse = await _httpClient.GetFromJsonAsync<APIResponse<ExamTypeGeneralDisplayDTO>>($"{BaseUrl}/ExamTypes/get-all-exam-types");
                ViewBag.ExamTypes = new SelectList(examTypesResponse.Result, "ExamTypeId", "ExamTypeName");

                var usersResponse = await _httpClient.GetFromJsonAsync<APIResponse<UserDisplayDTO>>($"{BaseUrl}/Users/get-all-users");
                var host = usersResponse?.IsSuccess == true && usersResponse.Result != null
                    ? usersResponse.Result.Where(user => user.IsActive && user.RoleName != "Admin").ToList()
                    : new List<UserDisplayDTO>();
                ViewBag.Hosts = new SelectList(host, "UserId", "UserName");

                return View(request);
            }

            // Lấy RequestedUserId từ token JWT
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
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";

            // Gửi request đến API edit-exam
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/Exams/edit-exam", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<APIResponse<object>>();
                if (result != null && result.IsSuccess)
                {
                    TempData["SuccessMessage"] = result.Message ?? AppMessages.EXAM_UPDATE_SUCCESS;
                    return RedirectToAction(nameof(Index));
                }
            }

            // Xử lý lỗi
            var errorResponse = await response.Content.ReadFromJsonAsync<APIResponse<object>>();
            ModelState.AddModelError("", errorResponse?.Message ?? AppMessages.INTERNAL_SERVER_ERROR);

            // Tải lại danh sách ExamTypes và Users
            var examTypesResponseRetry = await _httpClient.GetFromJsonAsync<APIResponse<ExamTypeGeneralDisplayDTO>>($"{BaseUrl}/ExamTypes/get-all-exam-types");
            ViewBag.ExamTypes = examTypesResponseRetry?.IsSuccess == true && examTypesResponseRetry.Result != null
                ? new SelectList(examTypesResponseRetry.Result, "ExamTypeId", "ExamTypeName")
                : new SelectList(new List<ExamTypeGeneralDisplayDTO>(), "ExamTypeId", "ExamTypeName");

            var usersResponseRetry = await _httpClient.GetFromJsonAsync<APIResponse<UserDisplayDTO>>($"{BaseUrl}/Users/get-all-users");
            var hosts = usersResponseRetry?.IsSuccess == true && usersResponseRetry.Result != null
                ? usersResponseRetry.Result.Where(user => user.IsActive && user.RoleName != "Admin").ToList()
                : new List<UserDisplayDTO>();
            ViewBag.Hosts = new SelectList(hosts, "UserId", "UserName");

            return View(request);
        }

    }
}
