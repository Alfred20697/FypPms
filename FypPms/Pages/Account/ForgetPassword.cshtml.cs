using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FypPms.Models;
using FypPms.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages.Account
{
    public class ForgetPasswordModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ForgetPasswordModel> _logger;
        private readonly IEmailSender _emailSender;

        public new User User { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ForgetPasswordModel(FypPmsContext context, ILogger<ForgetPasswordModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        public class ForgetPasswordData
        {
            [Required]
            [DisplayName("User Name")]
            public string UserName { get; set; }
        }

        [BindProperty]
        public ForgetPasswordData forgetPasswordData { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            User = await _context.User.FirstOrDefaultAsync(s => s.UserName == forgetPasswordData.UserName);
            bool TryAgain = false;

            if (User != null)
            {
                if (User.UserType == "Student")
                {

                    var student = await _context.Student.Where(s => s.DateDeleted == null && s.StudentStatus == "On").FirstOrDefaultAsync(s => s.AssignedId == User.UserName);

                    if (student != null)
                    {
                        var newPassword = RandomPassword();
                        var hashedPassword = HashPassword(newPassword, student.AssignedId);

                        User.UserPassword = hashedPassword;
                        User.DateModified = DateTime.Now;

                        await _context.SaveChangesAsync();

                        var coordinator = await _context.Coordinator.Where(s => s.DateDeleted == null).FirstOrDefaultAsync();

                        // Email Handler
                        var mailsubject = "FYP System - Reset Login Credentials";

                        var mailbody = $@"Dear {student.StudentName}, 

Your reset login credentials:

User Name: {student.AssignedId}
Password: {newPassword}

Please change your password at Settings. 

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{coordinator.CoordinatorName}
(FYP Coordinator)";

                        await _emailSender.SendEmailAsync(coordinator.CoordinatorEmail, student.StudentEmail, mailsubject, mailbody);

                        SuccessMessage = $"Password reset sent to student {User.UserName} email";
                    }
                    else
                    {
                        ErrorMessage = $"Student {User.UserName} not found";
                        TryAgain = true;
                    }
                }
                if (User.UserType == "Supervisor")
                {
                    var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null && s.SupervisorStatus == "Active").FirstOrDefaultAsync(s => s.AssignedId == User.UserName);

                    if (supervisor != null)
                    {
                        var newPassword = RandomPassword();
                        var hashedPassword = HashPassword(newPassword, supervisor.AssignedId);

                        User.UserPassword = hashedPassword;
                        User.DateModified = DateTime.Now;

                        await _context.SaveChangesAsync();

                        var coordinator = await _context.Coordinator.Where(s => s.DateDeleted == null).FirstOrDefaultAsync();

                        // Email Handler
                        var mailsubject = "FYP System - Login Credentials";

                        var mailbody = $@"Dear {supervisor.SupervisorName}, 

Your reset login credentials:

User Name: {supervisor.AssignedId}
Password: {newPassword}

Please change your password at Settings. 

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{coordinator.CoordinatorName}
(FYP Coordinator)";

                        await _emailSender.SendEmailAsync(coordinator.CoordinatorEmail, supervisor.SupervisorEmail, mailsubject, mailbody);

                        SuccessMessage = $"Password reset sent to supervisor {User.UserName} email";
                    }
                    else
                    {
                        ErrorMessage = $"Supervisor {User.UserName} not found";
                        TryAgain = true;
                    }
                }
                if (User.UserType == "Coordinator")
                {
                    // handler for coordinator
                    ErrorMessage = "Feature not available for coordinator";
                }
            }
            else
            {
                ErrorMessage = $"{forgetPasswordData.UserName} does not exist.";
                TryAgain = true;
            }

            if (!TryAgain)
            {
                return RedirectToPage("/Account/Login");
            }
            else
            {
                return RedirectToPage("/Account/ForgetPassword");
            }

        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("/Account/Login");
        }

        public string RandomPassword()
        {
            return Password.GenerateRandomPassword();
        }

        public string HashPassword(string password, string id)
        {
            return Hash.Generate(password, Salt.Generate(id)); ;
        }
    }
}