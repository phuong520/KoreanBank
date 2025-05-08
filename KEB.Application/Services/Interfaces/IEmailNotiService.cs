using KEB.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface IEmailNotiService
    {
        Task SendEmails(List<SendEmail> requests);

    }
}
