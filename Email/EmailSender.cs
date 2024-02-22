using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Transactions;

namespace Email
{

    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {


            //string fromMail = "internationalhorsecarecenter@gmail.com";
            //string fromPassword = "cpqswigitarlpleq";
            //string fromMail = "mashaersite@gmail.com";
            //string fromPassword = "mxkgpaqpzawlmucc";
            string fromMail = "info@mashaer.store";
            string fromPassword = "haxgudnayqqpsirl";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = subject;
            message.Body = $"<html><body>{htmlMessage}</body></html>";
            message.IsBodyHtml = true;
            message.To.Add(email);

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,


            };

            smtpClient.Send(message);
        }

        public Task SendEmailAsync(Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging.Message message)
        {
            throw new System.NotImplementedException();
        }
    }
}
