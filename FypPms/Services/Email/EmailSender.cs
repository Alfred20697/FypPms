using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FypPms.Services
{
    public class EmailSender : IEmailSender
    {
        private string host;
        private int port;
        private bool enableSSL;
        private string userName;
        private string password;

        // Get our parameterized configuration
        public EmailSender(string host, int port, bool enableSSL, string userName, string password)
        {
            this.host = host;
            this.port = port;
            this.enableSSL = enableSSL;
            this.userName = userName;
            this.password = password;
        }

        public async Task SendEmailAsync(string sender, string receiver, string mailsubject, string mailbody)
        {
            using (var message = new MailMessage(sender, receiver))
            {
                message.To.Add(new MailAddress(receiver));
                message.From = new MailAddress(sender);
                message.Subject = mailsubject;
                message.Body = mailbody;
                //message.IsBodyHtml = true;

                var smtpClient = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(userName, password),
                    EnableSsl = enableSSL,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                await smtpClient.SendMailAsync(message);
            }
        }
    }
}
