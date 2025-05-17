using KEB.Application.DTOs.ImportQuestionTaskDTO;
using KEB.Application.DTOs.StatisticDTO;
using KEB.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.MSIdentity.Shared;
using Newtonsoft.Json;

namespace KEB.WebApp.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/Statistics";
        private const string BaseApiUrl = "https://localhost:7101/api";

        public StatisticsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
       
    }
}

