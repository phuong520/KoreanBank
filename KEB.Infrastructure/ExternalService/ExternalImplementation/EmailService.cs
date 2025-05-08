using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using MailKit.Net.Smtp;
using System.Text;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using KEB.Infrastructure.ExternalService.IExternalInterfaces;

namespace KEB.Infrastructure.ExternalService.IExternalImplementation
{
    public class EmailService :IEmailService
    {
        public void SendEmail(string toEmail, string subject, string body, string fullName)
        {
            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse("htpchannel2106@gmail.com"));
            email.To.Add(new MailboxAddress(fullName, toEmail));

            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls); ;

            smtp.Authenticate("htpchannel2106@gmail.com", "fwro lzjh jpcp deda");

            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
