using KEB.Application.DTOs.QuestionTypeDTO;
using KEB.Application.DTOs.ReferenceDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;

namespace KEB.WebApp.Controllers
{
    public class QuestionTypeController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/QuestionTypes";

        public QuestionTypeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<IActionResult> Index()
        {
            var result = await _httpClient.GetFromJsonAsync<APIResponse<QuestionTypeDisplayDto>>($"{ApiUrl}/get-all-questiontypes");
            return View(result.Result);
        }
    }
}
