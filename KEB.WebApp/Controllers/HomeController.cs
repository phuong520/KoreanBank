using KEB.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KEB.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorViewModel = new ErrorViewModel
            {
                // Kiểm tra nếu `Activity.Current?.Id` null, sử dụng `HttpContext.TraceIdentifier`
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            if (string.IsNullOrEmpty(errorViewModel.RequestId))
            {
                errorViewModel.RequestId = "Không có ID yêu cầu để hiển thị."; // Dự phòng nếu không có RequestId
            }

            return View(errorViewModel);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
