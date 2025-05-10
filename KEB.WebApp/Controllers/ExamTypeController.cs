using KEB.Application.DTOs.ExamTypeDTO;
using KEB.Application.DTOs.LevelDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Mvc;
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
        
    }
}
