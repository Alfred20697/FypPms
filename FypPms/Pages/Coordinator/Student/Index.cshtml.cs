using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FypPms.Models;
using FypPms.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages.Coordinator.Student
{
    public class IndexModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<IndexModel> _logger;
        private readonly IEmailSender _emailSender;

        public IList<Models.Student> Students { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public IndexModel(FypPmsContext context, ILogger<IndexModel> logger, IEmailSender emailSender)
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
                    Students = await _context.Student
                        .Where(s => s.DateDeleted == null)
                        .Include(s => s.Batch)
                        .Include(s => s.Project)
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

        public async Task<IActionResult> OnPostOffAsync(int id)
        {
            var student = await _context.Student
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                ErrorMessage = "Student not found";
                return RedirectToPage("/Coordinator/Student/Index");
            }

            student.StudentStatus = "Off";
            student.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            await SendEmailAsync(student, "Off");

            SuccessMessage = $"Student {student.StudentName} status changed to Off successfully";

            return RedirectToPage("/Coordinator/Student/Index");
        }

        public async Task<IActionResult> OnPostOnAsync(int id)
        {
            var student = await _context.Student
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                ErrorMessage = "Student not found";
                return RedirectToPage("/Coordinator/Student/Index");
            }

            student.StudentStatus = "On";
            student.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            await SendEmailAsync(student, "On");

            SuccessMessage = $"Student {student.StudentName} status changed to On successfully";

            return RedirectToPage("/Coordinator/Student/Index");
        }

        public async Task<IActionResult> OnPostContinueAsync(int id)
        {
            var student = await _context.Student
                .Include(s => s.Project)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                ErrorMessage = "Student not found";
                return RedirectToPage("/Coordinator/Student/Index");
            }

            student.StudentStatus = "Continue";
            student.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            student.Project.ProjectStage = "FYP2";
            student.Project.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            await SendEmailAsync(student, student.Project, "Continue", "FYP 2");

            SuccessMessage = $"Student {student.StudentName} status changed to Continue successfully. Project {student.Project.AssignedId} changed to FYP 2 successfully.";

            return RedirectToPage("/Coordinator/Student/Index");
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            var student = await _context.Student
                .Include(s => s.Project)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                ErrorMessage = "Student not found";
                return RedirectToPage("/Coordinator/Student/Index");
            }

            student.StudentStatus = "Completed";
            student.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            student.Project.ProjectStatus = "Completed";
            student.Project.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            await SendEmailAsync(student, student.Project, "Completed", "Completed");

            SuccessMessage = $"Student {student.StudentName} status changed to Completed successfully. Project {student.Project.AssignedId} changed to Completed successfully.";

            return RedirectToPage("/Coordinator/Student/Index");
        }

        public async Task<IActionResult> OnPostFailAsync(int id)
        {
            var student = await _context.Student
                .Include(s => s.Project)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                ErrorMessage = "Student not found";
                return RedirectToPage("/Coordinator/Student/Index");
            }

            student.StudentStatus = "Failed";
            student.ProjectId = null;
            student.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            student.Project.ProjectStatus = "Available";
            student.Project.ProjectStage = null;
            student.Project.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            await SendEmailAsync(student, student.Project, "Failed", "Available");

            SuccessMessage = $"Student {student.StudentName} status changed to Failed successfully. Project {student.Project.AssignedId} changed to Available successfully.";

            return RedirectToPage("/Coordinator/Student/Index");
        }

        //public async Task<IActionResult> OnPostUncompleteAsync(int id)
        //{
        //    var student = await _context.Student
        //        .Include(s => s.Project)
        //        .FirstOrDefaultAsync(s => s.StudentId == id);

        //    if (student == null)
        //    {
        //        ErrorMessage = "Student not found";
        //        return RedirectToPage("/Coordinator/Student/Index");
        //    }

        //    student.StudentStatus = "Confirmed";
        //    student.DateModified = DateTime.Now;
        //    await _context.SaveChangesAsync();

        //    student.Project.ProjectStatus = "Taken";
        //    student.Project.DateModified = DateTime.Now;
        //    await _context.SaveChangesAsync();

        //    await SendEmailAsync(student, student.Project, "Confirmed", "Taken");

        //    SuccessMessage = $"Student {student.StudentName} status changed to Confirmed successfully. Project {student.Project.AssignedId} changed to Taken successfully.";

        //    return RedirectToPage("/Coordinator/Student/Index");
        //}

        public async Task SendEmailAsync(Models.Student student, string studentStatus)
        {
            var username = HttpContext.Session.GetString("_username");

            var coordinator = await _context.Coordinator.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(c => c.AssignedId == username);

            var mailsubject = "FYP System - Status Update";
            var mailbody = $@"Dear {student.StudentName}, 

Your status in the FYP System had changed to: {studentStatus}

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{coordinator.CoordinatorName}
(FYP Coordinator)";

            await _emailSender.SendEmailAsync(coordinator.CoordinatorEmail, student.StudentEmail, mailsubject, mailbody);
        }

        public async Task SendEmailAsync(Models.Student student, Models.Project project, string studentStatus, string projectStatus)
        {
            var username = HttpContext.Session.GetString("_username");

            var coordinator = await _context.Coordinator.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(c => c.AssignedId == username);

            var mailsubject = "FYP System - Status Update";
            var mailbody = $@"Dear {student.StudentName}, 

Your status in the FYP System had changed to: {studentStatus}

Your following project had changed to: {projectStatus}
Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{coordinator.CoordinatorName}
(FYP Coordinator)";

            await _emailSender.SendEmailAsync(coordinator.CoordinatorEmail, student.StudentEmail, mailsubject, mailbody);
        }
    }
}