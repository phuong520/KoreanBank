using KEB.Application.DTOs.ExamTypeDTO;
using KEB.Application.DTOs.ImportQuestionTaskDTO;
using KEB.Application.DTOs.LevelDTO;
using KEB.Application.DTOs.QuestionTypeDTO;
using KEB.Application.DTOs.ReferenceDTO;
using KEB.Application.DTOs.TopicDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Policy;

namespace KEB.WebApp.Controllers
{
    public class ExamTypeController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/ExamTypes";
        private const string BaseApiUrl = "https://localhost:7101/api";
        public ExamTypeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<IActionResult>  Index(int page = 1, int size = 10)
        {
            var queryParams = $"?page={page}&size={size}";

            var response = await _httpClient.GetFromJsonAsync<APIResponse<ExamTypeGeneralDisplayDTO>>($"{ApiUrl}/get-all-exam-types{queryParams}");

            if (response == null || !response.IsSuccess)
            {
                return View(new List<ExamTypeGeneralDisplayDTO>());
            }

            ViewBag.Page = page;
            ViewBag.Size = size;
            ViewBag.TotalCount = response.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)response.TotalCount / size);
            return View(response.Result);
        }
        private async Task LoadDropdownData()
        {
            var levels = await _httpClient.GetFromJsonAsync<APIResponse<LevelDisplayBriefDTO>>($"{BaseApiUrl}/Levels/get-all-levels");
            var types = await _httpClient.GetFromJsonAsync<APIResponse<QuestionTypeDisplayDto>>($"{BaseApiUrl}/QuestionTypes/get-all-questiontypes");
            var refs = await _httpClient.GetFromJsonAsync<APIResponse<ReferenceDisplayDto>>($"{BaseApiUrl}/References/get-all-references");
            var tasks = await _httpClient.GetFromJsonAsync<APIResponse<TaskGeneralDisplayDTO>>($"{BaseApiUrl}/Tasks/view-import-question-tasks");
            var topiclist = await _httpClient.GetFromJsonAsync<APIResponse<TopicDisplayDto>>($"{BaseApiUrl}/Topics");
            if (!levels.IsSuccess)
            {
                ModelState.AddModelError("", $"Không thể tải dữ liệu chủ đề: {levels.StatusCode}");

            }

            if (!types.IsSuccess)
            {
                ModelState.AddModelError("", $"Không thể tải dữ liệu dạng câu hỏi: {types.StatusCode}");

            }

            if (!refs.IsSuccess)
            {
                ModelState.AddModelError("", $"Không thể tải dữ liệu nguồn tham khảo: {refs.StatusCode}");

            }

            if (!tasks.IsSuccess)
            {
                ModelState.AddModelError("", $"Không thể tải dữ liệu nhiệm vụ: {tasks.StatusCode}");
            }

            if (levels == null || types == null || refs == null || tasks == null)
            {
                ModelState.AddModelError("", "Dữ liệu trả về từ API không hợp lệ");

            }

            ViewBag.Levels = new SelectList(levels.Result, "LevelId", "LevelName");
            ViewBag.QuestionTypes = new SelectList(types.Result, "QuestionTypeId", "QuestionTypeName");
            ViewBag.References = new SelectList(refs.Result, "Id", "ReferenceName");
            ViewBag.Tasks = new SelectList(tasks.Result, "Id", "TaskName");
            ViewBag.Topics = new SelectList(topiclist.Result, "TopicId", "TopicName");
        }
        public async Task<IActionResult> Create()
        {
            await LoadDropdownData();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(AddExamTypeRequest request)
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
            request.RequestedUserId = userId;
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var response = await _httpClient.PostAsJsonAsync($"{ApiUrl}/add-new-exam-type", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<APIResponse<ExamTypeComplexDisplayDTO>>();
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
