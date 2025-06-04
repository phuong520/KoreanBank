using KEB.Application.DTOs.NotificationDTO;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace KEB.WebApp.Controllers
{
    public class NotificationController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://localhost:7101/api/Notification"; // Replace with actual API URL

        public NotificationController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<IActionResult> Index()
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
                // Call API to get 7 latest notifications
                var notifications = await _httpClient.GetFromJsonAsync<List<NotificationDisplayDto>>($"{ApiUrl}/get-all/{userId}");

                if (notifications == null || notifications.Count == 0)
                {
                    ViewBag.Message = "No notifications found.";
                    return View(new List<NotificationDisplayDto>());
                }

                //TempData["Notifications"] = notifications;
                return View(notifications);
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the API call
                ViewBag.Message = $"Error: {ex.Message}";
                return View(new List<NotificationDisplayDto>());
            }
        }
        public async Task<IActionResult> Get7LatestNoti(Guid userId)
        {
            try
            {
                // Call API to get 7 latest notifications
                var notifications = await _httpClient.GetFromJsonAsync<List<NotificationDisplayDto>>($"{ApiUrl}/get-7-latest/{userId}");

                if (notifications == null || notifications.Count == 0)
                {
                    ViewBag.Message = "No notifications found.";
                    return View(new List<NotificationDisplayDto>());
                }

                TempData["Notifications"] = notifications;
                return View(notifications);
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the API call
                ViewBag.Message = $"Error: {ex.Message}";
                return View(new List<NotificationDisplayDto>());
            }
        }
        public async Task<IActionResult> MarkAllAsRead(Guid userId)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync<List<NotificationDisplayDto>>($"{ApiUrl}/mark-all-as-read/{userId}", null);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Message = "All notifications marked as read.";
                }
                else
                {
                    ViewBag.Message = "Error marking notifications as read.";
                }

                return RedirectToAction("Get7LatestNoti", new { userId });
            }
            catch (Exception ex)
            {
                // Handle any errors
                ViewBag.Message = $"Error: {ex.Message}";
                return RedirectToAction("Get7LatestNoti", new { userId });
            }
        }
    }
}
