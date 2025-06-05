using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ExamDTO;
using KEB.Application.DTOs.ExamPaperDTO;
using KEB.Application.DTOs.LevelDTO;
using KEB.Application.DTOs.QuestionDTO;
using KEB.Application.DTOs.SystemAccessLogDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace KEB.WebApp.Controllers
{
    public class PaperController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/ExamPapers";
        private const string BaseApiUrl = "https://localhost:7101/api";
        public PaperController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApiClient");
        }
        public async Task<IActionResult> Index(ViewExamPapersListRequest request)
        {
            try
            {

                var token = HttpContext.Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Common", new { returnUrl = Url.Action("Index", "Paper") });
                }
                
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                //await LoadDropdownData();
                var response = await _httpClient.GetFromJsonAsync<APIResponse<ExamAsTaskDisplayDTO>>($"{BaseApiUrl}/Tasks/view-exam-as-task");
                ViewBag.Exams = new SelectList(response.Result, "ExamId", "ExamName");

                request.PaginationRequest = new Pagination { Page = 1, Size = 20 };
                var query = $"?pageNumber={request.PaginationRequest.Page}&pageSize={request.PaginationRequest.Size}";

                //var query = $"?examId={request.ExamId}&levelId={request.LevelId}&nameValueTobeSearched={request.NameValueToBeSearched}&pageNumber={request.PaginationRequest.Page}&pageSize={request.PaginationRequest.Size}";

                var apiResponse = await _httpClient.GetFromJsonAsync<APIResponse<PaperGeneralDisplayDTO>>($"{ApiUrl}/get-papers{query}");

                if (apiResponse.IsSuccess)
                {
                    ViewBag.Page = apiResponse.Pagination.Page;
                    ViewBag.Size = apiResponse.Pagination.Size;
                    ViewBag.TotalCount = apiResponse.TotalCount;
                    ViewBag.TotalPages = (int)Math.Ceiling((double)apiResponse.TotalCount / apiResponse.Pagination.Size);
                    return View(apiResponse.Result);
                }

                TempData["ErrorMessage"] = "Không lấy được dữ liệu câu hỏi.";
                return View(new List<PaperGeneralDisplayDTO>());
            }
            catch (Exception ex)
            {
                {
                    TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                    return View(new List<PaperGeneralDisplayDTO>());
                }
            }
        }
        public async Task<IActionResult> Create(Guid examId, Guid requestedUserId, string? ipAddress)
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
            requestedUserId = userId1;
            ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var queryParams = $"?examId={examId}&requestedUserId={requestedUserId}&ipAddress={ipAddress}";

            var response = await _httpClient.GetFromJsonAsync<APIResponse<PaperDetailDisplayDTO>> ($"{ApiUrl}/gen-papers-for-exam{queryParams}");
            if (response.IsSuccess)
            {
                
                TempData["Success"] = "Tạo đề thi thành công!";
                return RedirectToAction(nameof(Index));
            }
            Console.WriteLine(response.Message + response.Result);
            ModelState.AddModelError("", "Không thể tạo loại câu hỏi. Vui lòng thử lại.");
            TempData["ErrorMessage"] = "Không thể tạo loại câu hỏi. Vui lòng thử lại.";
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Detail(Guid paperId)
        {
            // Get the token and extract user ID
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

            // Prepare the request URL with query parameters
            string requestUrl = $"{ApiUrl}/get-paper-detail?requestedUserId={userId}&paperId={paperId}";

            // Call the API with GET method
            var apiResponse = await _httpClient.GetAsync(requestUrl);
            var response = await apiResponse.Content.ReadFromJsonAsync<APIResponse<PaperDetailDisplayDTO>>();

            if (response?.IsSuccess == true && response.Result.Any())
            {
                var paperDetail = response.Result.First();
                return View(paperDetail); // Pass data to the view
            }

            // Error handling
            TempData["ErrorMessage"] = response?.Message ?? "Không thể tải đề thi. Vui lòng thử lại.";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> ViewPaperActivities(Guid paperId)
        {
            var response = await _httpClient.GetFromJsonAsync<APIResponse<AccessLogDisplayDto>>($"{ApiUrl}/view-activities-on-paper");
            if (response.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Không thể tạo loại câu hỏi. Vui lòng thử lại.");
            TempData["ErrorMessage"] = "Không thể tạo loại câu hỏi. Vui lòng thử lại.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PreviewPaper(Guid paperId)
        {
            // Get the token and extract user ID
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

            // Prepare the request URL with query parameters
            string requestUrl = $"{ApiUrl}/get-paper-detail?requestedUserId={userId}&paperId={paperId}";

            // Call the API with GET method
            var apiResponse = await _httpClient.GetAsync(requestUrl);
            var response = await apiResponse.Content.ReadFromJsonAsync<APIResponse<PaperDetailDisplayDTO>>();

            if (response?.IsSuccess == true && response.Result.Any())
            {
                var paperDetail = response.Result.First();
                return View(paperDetail); // Pass data to the view
            }

            // Error handling
            TempData["ErrorMessage"] = response?.Message ?? "Không thể tải đề thi. Vui lòng thử lại.";
            return RedirectToAction("Index");
        }
    }
    
}
