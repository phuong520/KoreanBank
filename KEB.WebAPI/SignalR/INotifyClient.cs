using KEB.Application.DTOs.NotificationDTO;

namespace KEB.WebAPI.SignalR
{
    public interface INotifyClient
    {
        Task SendLatestNotifications(List<NotificationDisplayDto> notis);
    }
}
