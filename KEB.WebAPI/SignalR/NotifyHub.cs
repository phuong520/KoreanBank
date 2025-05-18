using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using KEB.Application.Services;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace KEB.WebAPI.SignalR
{
    public class NotifyHub : Hub<INotifyClient>
    {
        private readonly IUnitOfService _unitOfService;
        private readonly Dictionary<string, string> _connectedUsers = [];

        public NotifyHub(IUnitOfService unitOfService)
        {
            _unitOfService = unitOfService;
        }
        public override async Task OnConnectedAsync()
        {
            if (Context.User?.Identity?.IsAuthenticated ?? false)
            {
                var userId = Context.User.FindFirst(ClaimTypes.Sid)?.Value;
                Console.WriteLine($"User connected: UserId = {userId}");

                if (userId == null)
                {
                    Console.WriteLine("Unauthenticated connection attempt.");
                    Context.Abort();
                }
                else
                {
                    _connectedUsers.Add(userId, Context.ConnectionId);
                }
                await Groups.AddToGroupAsync(Context.ConnectionId, "KEB Noti");
            }
            else
            {
                Console.WriteLine("Unauthenticated connection attempt.");
                Context.Abort();
            }
            await base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _connectedUsers.Remove(Context.User?.FindFirst(ClaimTypes.Sid)?.Value ?? "");
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
       
        public async Task<bool> GetLatestNoti()
        {
            try
            {
                var userIdClaim = Context.User?.FindFirst(ClaimTypes.Sid)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    await Console.Out.WriteLineAsync("Invalid or missing userId");
                    return false;
                }

                var notis = await _unitOfService.NotiService.Get7LatestNoti(userId);
                await Clients.Caller.SendLatestNotifications(notis);
                return true;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error in GetLatestNoti: {ex.Message}");
                throw; // hoặc return false nếu muốn
            }
        }

    }
}
