using KEB.Application.DTOs.NotificationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface INotiService
    {
        Task<List<NotificationDisplayDto>> GetAllNoti(Guid userId);
        Task<List<NotificationDisplayDto>> Get7LatestNoti(Guid userId);
        Task<bool> MarkAsRead(Guid notiId);
        Task<bool> MarkAllNotiAsRead(Guid userId);
    }
}
