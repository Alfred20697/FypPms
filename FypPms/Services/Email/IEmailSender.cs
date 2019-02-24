using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FypPms.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string sender, string receiver, string mailsubject, string mailbody);
    }
}
