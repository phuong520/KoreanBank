using AutoMapper;
using KEB.Application.DTOs.Common;
using KEB.Application.Services.Interfaces;
using KEB.Infrastructure.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Implementations
{
    public class EmailNotiService : IEmailNotiService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmailNotiService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
        }

        public async Task SendEmails(List<SendEmail> requests)
        {
            var tasks = requests.Select(request => Task.Run(() => SendInformEmail(request)));
            await Task.WhenAll(tasks);
        }
        private void SendInformEmail(SendEmail? request)
        {
            if (request == null) return;
            try
            {
                _unitOfWork.EmailService.SendEmail(request.Email,
                                                    request.Subject,
                                                    request.Body,
                                                    request.RecipientFullName);
            }
            catch (Exception) { }
        }
    }
}
