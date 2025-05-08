using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;
using DocumentFormat.OpenXml.Office2021.Drawing.SketchyShapes;
using KEB.Application.DTOs.AnswerDTO;
using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ExamDTO;
using KEB.Application.DTOs.ImportQuestionTaskDTO;
using KEB.Application.DTOs.LevelDTO;
using KEB.Application.DTOs.LevelTopicDetailDTO;
using KEB.Application.DTOs.QuestionAddDTO;
using KEB.Application.DTOs.QuestionDTO;
using KEB.Application.DTOs.QuestionTypeDTO;
using KEB.Application.DTOs.ReferenceDTO;
using KEB.Application.DTOs.TopicDTO;
using KEB.Application.Services;
using KEB.Domain.Entities;
using KEB.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace KEB.WebApp.Controllers
{
    public class QuestionController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/Questions";
        private const string BaseApiUrl = "https://localhost:7101/api";

        public QuestionController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            try
            {

                var token = HttpContext.Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Common", new { returnUrl = Url.Action("Create", "Question") });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Tạo request query
                var request = new GetQuestionsRequest
                {
                    PaginationRequest = new Pagination
                    {
                        Page = 1,
                        Size = 20
                    },
                    Status = new List<QuestionStatus>
                    {
                        QuestionStatus.Pending,
                        QuestionStatus.Ok
                    }, // ví dụ lọc theo trạng thái
                    SortAscending = false
                };

                var queryParams = new Dictionary<string, string?>
                {
                    ["Page"] = request.PaginationRequest?.Page.ToString(),
                    ["Size"] = request.PaginationRequest?.Size.ToString(),
                    ["SortAscending"] = request.SortAscending.ToString()
                    // Thêm các param khác nếu cần
                };

                var url = QueryHelpers.AddQueryString($"{ApiUrl}/get-questions", queryParams);

                var result = await _httpClient.GetFromJsonAsync<APIResponse<QuestionDisplayDto>>(url);

                if (result?.IsSuccess == true)
                {
                    return View(result.Result);
                }

                TempData["ErrorMessage"] = result?.Message ?? "Không thể tải danh sách câu hỏi";
                return View(new List<QuestionDisplayDto>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách câu hỏi: " + ex.Message;
                return View(new List<QuestionDisplayDto>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                //SetAuthorizationHeader();

                var response = await _httpClient.GetAsync($"{ApiUrl}/get-question-has-id-{id}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<APIResponse<QuestionDetailDto>>();
                    if (result?.IsSuccess == true && result.Result.Count > 0)
                    {
                        return View(result.Result.FirstOrDefault());
                    }
                }

                TempData["ErrorMessage"] = "Không tìm thấy câu hỏi hoặc có lỗi xảy ra";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải chi tiết câu hỏi: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<APIResponse<DetailDisplayDTO>> GetTopic(Guid levelId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseApiUrl}/LevelDetails/get-detail-by-level-id/{levelId}");

                if (!response.IsSuccessStatusCode)
                {
                    return new APIResponse<DetailDisplayDTO>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi gọi API nội bộ: {(int)response.StatusCode} {response.ReasonPhrase}"
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<APIResponse<DetailDisplayDTO>>();

                if (result == null)
                {
                    return new APIResponse<DetailDisplayDTO>
                    {
                        IsSuccess = false,
                        Message = "Không đọc được nội dung phản hồi từ API nội bộ"
                    };
                }

                return result;
            }
            catch (Exception ex)
            {
                return new APIResponse<DetailDisplayDTO>
                {
                    IsSuccess = false,
                    Message = $"Lỗi ngoại lệ: {ex.Message}"
                };
            }

        }
        private async Task LoadDropdownData()
        {
            var levels = await _httpClient.GetFromJsonAsync<APIResponse<LevelDisplayBriefDTO>>($"{BaseApiUrl}/Levels/get-all-levels");
            var types = await _httpClient.GetFromJsonAsync<APIResponse<QuestionTypeDisplayDto>>($"{BaseApiUrl}/QuestionTypes/get-all-questiontypes");
            var refs = await _httpClient.GetFromJsonAsync<APIResponse<ReferenceDisplayDto>>($"{BaseApiUrl}/References/get-all-references");
            var tasks = await _httpClient.GetFromJsonAsync<APIResponse<TaskGeneralDisplayDTO>>($"{BaseApiUrl}/Tasks/view-import-question-tasks");

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
        }
        // [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var token = HttpContext.Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Common", new { returnUrl = Url.Action("Create", "Question") });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                await LoadDropdownData();


                return View(new AddSingleQuestionRequest
                {
                    //RequestedUserId = GetCurrentUserId(),
                    IsMultipleChoice = true, // Mặc định là câu hỏi trắc nghiệm
                    Answers = new List<AddAnswerDTO>
                        {
                            new AddAnswerDTO(), new AddAnswerDTO(),
                            new AddAnswerDTO(), new AddAnswerDTO()
                        }
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
                return View();
            }
        }

        [HttpPost()]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddSingleQuestionRequest request)
        {
            if (ModelState.IsValid)
            {
                try

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

                    //// Tạo nội dung multipart/form-data
                    using var formData = new MultipartFormDataContent();

                    //// Thêm các field dạng text
                    request.RequestedUserId = userId;
                    formData.Add(new StringContent(request.LevelDetailId.ToString()), nameof(request.LevelDetailId));
                    formData.Add(new StringContent(request.ReferenceId.ToString()), nameof(request.ReferenceId));
                    formData.Add(new StringContent(request.QuestionTypeId.ToString()), nameof(request.QuestionTypeId));
                    formData.Add(new StringContent(request.RequestedUserId.ToString()), nameof(request.RequestedUserId));
                    formData.Add(new StringContent(((int)request.Difficulty).ToString()), nameof(request.Difficulty));
                    formData.Add(new StringContent(request.QuestionContent ?? string.Empty), nameof(request.QuestionContent));
                    //var answersJson = System.Text.Json.JsonSerializer.Serialize(request.Answers);
                    //formData.Add(new StringContent(answersJson, Encoding.UTF8, "application/json"), nameof(request.Answers));
                    // Thêm các đáp án
                    for (int i = 0; i < request.Answers.Count; i++)
                    {
                        var answer = request.Answers[i];
                        formData.Add(new StringContent(answer.Content ?? ""), $"Answers[{i}].Content");
                        formData.Add(new StringContent(answer.IsCorrect.ToString()), $"Answers[{i}].IsCorrect");
                    }

                    if (request.TaskId != null)
                        formData.Add(new StringContent(request.TaskId.ToString()), nameof(request.TaskId));
                    // Thêm file nếu có
                    if (request.AttachmentFile != null && request.AttachmentFile.Length > 0)
                    {
                        var streamContent = new StreamContent(request.AttachmentFile.OpenReadStream());
                        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(request.AttachmentFile.ContentType);
                        formData.Add(streamContent, nameof(request.AttachmentFile), request.AttachmentFile.FileName);
                    }

                    // Gửi request
                    var response = await _httpClient.PostAsync($"{ApiUrl}/add-single-question", formData);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content?.ReadAsStringAsync() ?? "Không có nội dung lỗi.";
                        ModelState.AddModelError("", $"API trả về lỗi: {(int)response.StatusCode} - {response.ReasonPhrase}\n{errorContent}");
                    }
                    var apiResult = await response.Content.ReadFromJsonAsync<APIResponse<QuestionDetailDto>>();

                    // Xử lý kết quả
                    if (response.IsSuccessStatusCode && apiResult?.IsSuccess == true)
                    {
                        TempData["SuccessMessage"] = "Thêm câu hỏi thành công!";
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError("", apiResult?.Message ?? "Không thể thêm câu hỏi.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi hệ thống: {ex.Message}");
                }
            }

            return View(request);

        }



    }
}