using KEB.Application.DTOs.ExamTypeDTO;
using KEB.Application.DTOs.ImportQuestionTaskDTO;
using KEB.Application.DTOs.LevelDTO;
using KEB.Application.DTOs.QuestionTypeDTO;
using KEB.Application.DTOs.ReferenceDTO;
using KEB.Application.DTOs.TopicDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult>  Index()
        {
            var result = await _httpClient.GetFromJsonAsync<APIResponse<ExamTypeGeneralDisplayDTO>>($"{ApiUrl}/get-all-exam-types");

            if (result == null || !result.IsSuccess)
            {
                return View(new List<object>());
            }

            return View(result.Result);
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
            LoadDropdownData();
            return View();
        }

    }
}
