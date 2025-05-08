using KEB.Application.DTOs.ExamDTO;
using KEB.Application.DTOs.ReferenceDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KEB.WebApp.Controllers
{
    public class ExamController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/Exams";

        public ExamController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task< IActionResult> Index()
        {
            
                var response = await _httpClient.GetFromJsonAsync<APIResponse<object>>($"{ApiUrl}/get-exam");
            return View(response.Result);
        }
    
    }
}
