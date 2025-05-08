using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.Common
{
    public class SendEmail
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string RecipientFullName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;

    }
}
