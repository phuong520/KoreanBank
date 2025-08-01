﻿using Azure;
using KEB.Application.DTOs.LevelDTO;
using KEB.Application.DTOs.TopicDTO;
using KEB.Application.DTOs.UserDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace KEB.WebApp.Controllers
{
    public class LevelsController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/Levels";
        private const string BaseApiUrl = "https://localhost:7101/api";

        public LevelsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<IActionResult> Index()
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
            var url = $"{ApiUrl}/get-all-levels";
            var topiclist = await _httpClient.GetFromJsonAsync<APIResponse<TopicDisplayDto>>($"{BaseApiUrl}/Topics");
            ViewBag.Topics = new SelectList(topiclist.Result, "TopicId", "TopicName");

            var result = await _httpClient.GetFromJsonAsync<APIResponse<LevelDisplayBriefDTO>>(url);

            if (result == null || !result.IsSuccess)
            {
                return View(new List<LevelDisplayBriefDTO>());
            }
            ViewBag.Page = result.Pagination.Page;
            ViewBag.Size = result.Pagination.Size;
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / result.Pagination.Size);
            return View(result.Result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddLevelRequest request, string ipAddress)
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

                request.RequestedUserId = userId;
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var response = await _httpClient.PostAsJsonAsync($"{ApiUrl}/add-new-level", request);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadFromJsonAsync<APIResponse<LevelDisplayDetailDTO>>();
                    // dùng responseData nếu cần
                    TempData["Success"] = "Tạo nhiệm vụ thành công!";
                    return RedirectToAction(nameof(Index));
                }
                return View(request);
            }

            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
                return View(request);
            }
        }
        [HttpPut]
        public async Task<IActionResult> Update()
        {

            return View();
        }

        public async Task<ActionResult> Details(Guid levelId)
        {
            var response = await _httpClient.GetFromJsonAsync<APIResponse<LevelDisplayDetailDTO>>($"{ApiUrl}/get-level-has-id-{levelId}");

            if (!response.IsSuccess)
            {
                return View("Error"); // hoặc trả về NotFound(), tùy bạn
            }

            return View(response.Result); // Truyền sang view để hiển thị
        }

    }


}
