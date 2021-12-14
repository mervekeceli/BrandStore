using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BrandStore.Areas.Identity.Data
{
    public class EmailSender:IEmailSender
    {
        public Task SendEmailAsync(string email,string subject,string htmlMessage)
        {
            MailMessage mail = new MailMessage
            {
                From = new MailAddress("sule.aktas1@ogr.sakarya.edu.tr", "BrandStore", System.Text.Encoding.UTF8),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };
            mail.To.Add(email);
            SmtpClient smp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Credentials = new NetworkCredential("sule.aktas1@ogr.sakarya.edu.tr", "10004259854"),
                Port = 587,
                EnableSsl = true,
            };
            smp.Send(mail);
            return Task.CompletedTask;
        }
    }
}
