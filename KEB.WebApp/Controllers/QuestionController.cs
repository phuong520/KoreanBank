using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;
using DocumentFormat.OpenXml.Office2021.Drawing.SketchyShapes;
using Grpc.Core;
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
using System.Net.Http;
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
        private IMapper _mapper;

        public QuestionController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        //[Authorize(Roles = "Giảng viên")]
        public async Task<IActionResult> Index(GetQuestionsRequest request)
        {
            try
            {

                var token = HttpContext.Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Commonweb", new { returnUrl = Url.Action("Create", "Question") });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                await LoadDropdownData();

                request.PaginationRequest = new Pagination { Page = 1, Size = 20 };
                request.SortAscending = false;
                request.Status = new List<QuestionStatus> { QuestionStatus.Ok };
                var response = await _httpClient.PostAsJsonAsync($"{ApiUrl}/get-questions", request); // request rỗng sẽ trả toàn bộ
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse<QuestionDisplayDto>>();
                    return View(apiResponse.Result);
                }

                TempData["ErrorMessage"] = "Không lấy được dữ liệu câu hỏi.";
                return View(new List<QuestionDisplayDto>());
            }
            catch (Exception ex)
            {
                {
                    TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                    return View(new List<QuestionDisplayDto>());
                }
            }
        }
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {

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
       
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var token = HttpContext.Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Commonweb", new { returnUrl = Url.Action("Create", "Question") });
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
                    formData.Add(new StringContent(request.AttachmentDuration?.ToString() ?? "0"), nameof(request.AttachmentDuration));
                    formData.Add(new StringContent(request.IsMultipleChoice.ToString()), nameof(request.IsMultipleChoice));

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
                   
                        if (request.AttachmentFileImage != null && request.AttachmentFileImage.Length > 0)
                        {
                            var streamContentImage = new StreamContent(request.AttachmentFileImage.OpenReadStream());
                            streamContentImage.Headers.ContentType = new MediaTypeHeaderValue(request.AttachmentFileImage.ContentType);
                            formData.Add(streamContentImage, nameof(request.AttachmentFileImage), request.AttachmentFileImage.FileName);
                        }

                        if (request.AttachmentFileAudio != null && request.AttachmentFileAudio.Length > 0)
                        {
                            var streamContentAudio = new StreamContent(request.AttachmentFileAudio.OpenReadStream());
                            streamContentAudio.Headers.ContentType = new MediaTypeHeaderValue(request.AttachmentFileAudio.ContentType);
                            formData.Add(streamContentAudio, nameof(request.AttachmentFileAudio), request.AttachmentFileAudio.FileName);
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
            await LoadDropdownData();
            return View(request);

        }
        
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var token = HttpContext.Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Commonweb", new { returnUrl = Url.Action("Edit", "Question", new { id }) });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Load data for the specified question
                var response = await _httpClient.GetAsync($"{ApiUrl}/get-question-has-id-{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                var apiResult = await response.Content.ReadFromJsonAsync<APIResponse<QuestionDetailDto>>();
                if (apiResult?.IsSuccess != true || apiResult.Result.Count == 0)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy câu hỏi hoặc bạn không có quyền chỉnh sửa";
                    return RedirectToAction(nameof(Index));
                }

                var questionDetail = apiResult.Result[0];
               
                // Map QuestionDetailDto to UpdateQuestionRequest
                var updateRequest = new UpdateQuestionRequest
                {
                    TargetObjectId = questionDetail.Id,
                    NewQuestionContent = questionDetail.QuestionContent,
                    NewDifficulty = questionDetail.Difficulty,
                    NewReferenceId = questionDetail.ReferenceId,
                    Answers = (List<AddAnswerDTO>)questionDetail.Answers, 
                    AttachmentChanged = false
                };
                // Store the current question details in ViewBag for comparison in the view
                ViewBag.CurrentQuestion = questionDetail;

                // Load dropdown data for the form
                await LoadDropdownData();

                return View(updateRequest);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateQuestionRequest request)
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
                using var formData = new MultipartFormDataContent();

                // Thêm các field dạng text
                request.RequestedUserId = userId;
                request.IpAddress = "";
                formData.Add(new StringContent(request.TargetObjectId.ToString()), nameof(request.TargetObjectId));
                formData.Add(new StringContent(request.RequestedUserId.ToString()), nameof(request.RequestedUserId));

                // Chỉ gửi các giá trị đã thay đổi
                if (!string.IsNullOrEmpty(request.NewQuestionContent))
                {
                    formData.Add(new StringContent(request.NewQuestionContent), nameof(request.NewQuestionContent));
                }

                if (request.NewDifficulty.HasValue)
                {
                    formData.Add(new StringContent(((int)request.NewDifficulty).ToString()), nameof(request.NewDifficulty));
                }

                if (request.NewReferenceId.HasValue)
                {
                    formData.Add(new StringContent(request.NewReferenceId.ToString()), nameof(request.NewReferenceId));
                }

                // Xử lý các đáp án nếu có thay đổi
                 if (request.AnswersChanged && request.Answers != null && request.Answers.Any())
                {
                    var answersJson = System.Text.Json.JsonSerializer.Serialize(request.Answers);
                    formData.Add(new StringContent(answersJson), nameof(request.Answers));
                    formData.Add(new StringContent(request.AnswersChanged.ToString()), nameof(request.AnswersChanged));
                }

                // Thêm file mới nếu có
                if (request.AttachmentChanged)
                {

                    // Xử lý Image Attachment
                    if (request.AttachmentFileImage != null && request.AttachmentFileImage.Length > 0)
                    {
                        var streamContentImage = new StreamContent(request.AttachmentFileImage.OpenReadStream());
                        streamContentImage.Headers.ContentType = new MediaTypeHeaderValue(request.AttachmentFileImage.ContentType);
                        formData.Add(streamContentImage, nameof(request.AttachmentFileImage), request.AttachmentFileImage.FileName);
                    }

                    // Xử lý Audio Attachment
                    if (request.AttachmentFileAudio != null && request.AttachmentFileAudio.Length > 0)
                    {
                        var streamContentAudio = new StreamContent(request.AttachmentFileAudio.OpenReadStream());
                        streamContentAudio.Headers.ContentType = new MediaTypeHeaderValue(request.AttachmentFileAudio.ContentType);
                        formData.Add(streamContentAudio, nameof(request.AttachmentFileAudio), request.AttachmentFileAudio.FileName);
                    }

                    // Thêm flag AttachmentChanged
                    formData.Add(new StringContent(request.AttachmentChanged.ToString()), nameof(request.AttachmentChanged));
                }

                // Thêm địa chỉ IP người dùng (nếu cần)
                string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    formData.Add(new StringContent(ipAddress), nameof(request.IpAddress));
                }

                // Gửi request
                var response = await _httpClient.PostAsync($"{ApiUrl}/edit-question", formData);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content?.ReadAsStringAsync() ?? "Không có nội dung lỗi.";
                    ModelState.AddModelError("", $"API trả về lỗi: {(int)response.StatusCode} - {response.ReasonPhrase}\n{errorContent}");
                }

                var apiResult = await response.Content.ReadFromJsonAsync<APIResponse<object>>();

                // Xử lý kết quả
                if (response.IsSuccessStatusCode && apiResult?.IsSuccess == true)
                {
                    TempData["SuccessMessage"] = "Cập nhật câu hỏi thành công!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", apiResult?.Message ?? "Không thể cập nhật câu hỏi.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi hệ thống: {ex.Message}");
            }

            // Nếu có lỗi, load lại dữ liệu dropdown và hiển thị form
            await LoadDropdownData();
            try
            {
                var token = HttpContext.Request.Cookies["token"];
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var detailResponse = await _httpClient.GetAsync($"{ApiUrl}//get-question-has-id-{request.TargetObjectId}");

                if (detailResponse.IsSuccessStatusCode)
                {
                    var apiDetailResult = await detailResponse.Content.ReadFromJsonAsync<APIResponse<QuestionDetailDto>>();
                    if (apiDetailResult?.IsSuccess == true && apiDetailResult.Result.Count > 0)
                    {
                        ViewBag.CurrentQuestion = apiDetailResult.Result[0];
                    }
                }
            }
            catch
            {
            }

            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadTemplate(bool forMultipleChoice = true)
        {
            var response = await _httpClient.GetAsync($"{ApiUrl}/download-template?forMultipleChoice={forMultipleChoice}");
            if (response.IsSuccessStatusCode)
            {
                var bytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = forMultipleChoice ? "multiple_choice_template.xlsx" : "essay_template.xlsx";
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            return BadRequest("Không thể tải template");
        }
        
       
        public async Task<IActionResult> ImportExcel()
        {
            await LoadDropdownData();
            return View(new ImportQuestionFromExcelRequest());
        }
        
        [HttpPost]
        public async Task<IActionResult> ImportExcel(ImportQuestionFromExcelRequest request)
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
            using var content = new MultipartFormDataContent();

            if (request.ExcelFile != null && request.ExcelFile.Length > 0)
            {
                var excelStream = request.ExcelFile.OpenReadStream();
                var excelContent = new StreamContent(excelStream);
                excelContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "ExcelFile",
                    FileName = request.ExcelFile.FileName
                };
                content.Add(excelContent);
            }
            else
            {
                ModelState.AddModelError("ExcelFile", "Excel file is required.");
                return View(request);
            }
            content.Add(new StringContent(request.ForMultipleChoice.ToString()), "ForMultipleChoice");
            if (request.TaskId.HasValue)
            {
                content.Add(new StringContent(request.TaskId.Value.ToString()), "TaskId");
            }
            content.Add(new StringContent(request.RequestedUserId.ToString()), "RequestedUserId");

            if (request.Attachments != null)
            {
                foreach (var attachment in request.Attachments.Where(a => a != null && a.Length > 0))
                {
                    var attachmentStream = attachment.OpenReadStream();
                    var attachmentContent = new StreamContent(attachmentStream);
                    attachmentContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "Attachments",
                        FileName = attachment.FileName
                    };
                    content.Add(attachmentContent);
                }
            }

            var response = await _httpClient.PostAsync($"{ApiUrl}/upload-excel", content);
            var responseJson = await response.Content.ReadAsStringAsync();
            var apiResponse = System.Text.Json.JsonSerializer.Deserialize<APIResponse<ImportQuestionResultDTO>>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (response.IsSuccessStatusCode && apiResponse.IsSuccess)
            {
                TempData["SuccessMessage"] = apiResponse.Message;
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", apiResponse.Message);
                if (apiResponse.Result.Any())
                {
                    ModelState.AddModelError("", string.Join("; ", apiResponse.Result.First().Messages));
                }
                return View(request);
            }
        
        }
    }
}