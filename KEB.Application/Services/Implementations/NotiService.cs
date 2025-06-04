using AutoMapper;
using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.NotificationDTO;
using KEB.Application.Services.Interfaces;
using KEB.Infrastructure.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Implementations
{
    public class NotiService : INotiService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public NotiService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<NotificationDisplayDto>> Get7LatestNoti(Guid userId)
        {
            List<NotificationDisplayDto> response = new List<NotificationDisplayDto>();
            try
            {
                var queried = await _unitOfWork.Notifications.GetAllAsync(x => x.UserId == userId,
                                            orderBy: x => x.OrderByDescending(x => x.CreatedDate));
                response = queried.Take(7).Select(x => new NotificationDisplayDto
                {
                    Id = x.Id,
                    //AnchorLink = x.AnchorLink,
                    CreatedTime = x.CreatedDate,
                    Description = x.Description,
                    IsRead = x.IsRead
                }).ToList();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
            return response;
        }

        public async Task<List<NotificationDisplayDto>> GetAllNoti(Guid userId)
        {
            List<NotificationDisplayDto> response = new List<NotificationDisplayDto>();
            try
            {

                var queried = await _unitOfWork.Notifications.GetAllAsync(x => x.UserId == userId,
                                            orderBy: x => x.OrderByDescending(x => x.CreatedDate));
                response = queried.Select(x => new NotificationDisplayDto
                {
                    Id = x.Id,
                    CreatedTime = x.CreatedDate,
                    Description = x.Description
                }).ToList();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
            return response;
        }

        public async Task<bool> MarkAllNotiAsRead(Guid userId)
        {
            try
            {
                var notifications = await _unitOfWork.Notifications.GetAllAsync(x => x.UserId == userId && !x.IsRead, asTracking: true);

                if (notifications.Any())
                {
                    foreach (var notification in notifications)
                    {
                        notification.IsRead = true;
                    }
                    
                    await _unitOfWork.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return false;
            }
        }

        public async Task<bool> MarkAsRead(Guid notiId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var notification = await _unitOfWork.Notifications.GetByIdAsync(notiId);
                if (notification != null && !notification.IsRead)
                {
                    notification.IsRead = true;
                    await _unitOfWork.Notifications.UpdateWithNoCommitAsync(notification);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return false;
            }
        }
    }
}
