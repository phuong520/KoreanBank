using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using KEB.Application.Services;
using Microsoft.AspNetCore.SignalR;

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
                var userId = Context.User.FindFirst("UserId")?.Value;
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
                await Groups.AddToGroupAsync(Context.ConnectionId, "FSAKEB Noti");
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
            _connectedUsers.Remove(Context.User?.FindFirst("UserId")?.Value ?? "");
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
        public async Task<bool> GetLatestNoti(Guid userId)
        {
            if (Clients == null)
            {
                await Console.Out.WriteLineAsync("Null Clients");
                return false;
            }
            var user = Clients.User(userId.ToString());
            if (user == null)
            {
                await Console.Out.WriteLineAsync("Null User");
                return false;
            }
            else
            {
                await Console.Out.WriteLineAsync("Called SendLatestNoti");
                var notis = await _unitOfService.NotiService.Get7LatestNoti(userId);
                await Clients.All.SendLatestNotifications(notis);
                return true;
            }
        }
    }
}
