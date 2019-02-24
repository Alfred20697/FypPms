using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FypPms.Models;
using FypPms.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace FypPms.Pages.Coordinator.Student
{
    public class StudentApprovalModel : PageModel
    {
        private readonly FypPmsContext _context;
        private ILogger<StudentApprovalModel> _logger;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public IList<Models.Student> Student { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public StudentApprovalModel(FypPmsContext context, ILogger<StudentApprovalModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Coordinator");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    Student = await _context.Student
                                .Where(s => s.DateDeleted == null)
                                .Where(s => s.StudentStatus == "New")
                                .Include(s => s.Batch)
                                .Include(s => s.User)
                                .ToListAsync();

                    return Page();
                }
                else
                {
                    ErrorMessage = "Access Denied";
                    return RedirectToPage($"/{usertype}/Index");
                }
            }
            else
            {
                ErrorMessage = "Login Required";
                return RedirectToPage("/Account/Login");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Update student status
            Models.Student student = await _context.Student
                                            .Include(s => s.Batch)
                                            .Include(s => s.User).FirstOrDefaultAsync(m => m.StudentId == id);

            student.StudentStatus = "On";
            student.DateModified = DateTime.Now;
            _context.Attach(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(student.StudentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var newPassword = RandomPassword();
            var hashedPassword = HashPassword(newPassword, student.AssignedId);

            // Create new user
            var newUser = new Models.User(student.AssignedId, hashedPassword, "Student")
            {
                DateCreated = DateTime.Now
            };
            _context.User.Add(newUser);
            await _context.SaveChangesAsync();

            var user = await _context.User.Where(u => u.DateDeleted == null).FirstOrDefaultAsync(u => u.UserName == student.AssignedId);
            
            student.UserId = user.UserId;
            await _context.SaveChangesAsync();

            var username = HttpContext.Session.GetString("_username");
            var coordinator = await _context.Coordinator.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(c => c.AssignedId == username);

            // Email Handler
            var mailsubject = "FYP System - Login Credentials";
            var mailbody = $@"Dear {student.StudentName}, 

Your login credentials:

User Name: {student.AssignedId}
Password: {newPassword}

Please change your password at Settings. 

Note : Account without any activity within 3 days after sending this email, may be deactivated.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{coordinator.CoordinatorName}
(FYP Coordinator)";

            await _emailSender.SendEmailAsync(coordinator.CoordinatorEmail, student.StudentEmail, mailsubject, mailbody);
            
            // Log Message
            SuccessMessage = $"Student {student.StudentName} approved!";

            return RedirectToPage("/Coordinator/Student/Index");
        }

        public string RandomPassword()
        {
            return Password.GenerateRandomPassword();
        }

        public string HashPassword(string password, string studentId)
        {
            return Hash.Generate(password, Salt.Generate(studentId)); ;
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.StudentId == id);
        }
    }
}