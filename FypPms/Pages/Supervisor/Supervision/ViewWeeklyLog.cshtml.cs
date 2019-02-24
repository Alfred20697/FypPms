using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FypPms.Models;
using FypPms.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages.Supervisor.Supervision
{
    public class ViewWeeklyLogModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ViewWeeklyLogModel> _logger;
        private readonly IEmailSender _emailSender;

        public Models.Student Student { get; set; }
        public Models.Supervisor Supervisor { get; set; }
        public Models.Supervisor CoSupervisor { get; set; }
        public WeeklyLog WeeklyLog { get; set; }
        [BindProperty]
        public ApprovalForm Af { get; set; }
        public bool CanApprove { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ViewWeeklyLogModel(FypPmsContext context, ILogger<ViewWeeklyLogModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        public class ApprovalForm
        {
            [DisplayName("Comment")]
            public string Comment { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Supervisor");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    WeeklyLog = await _context.WeeklyLog
                        .Where(w => w.DateDeleted == null)
                        .Include(w => w.Project)
                        .FirstOrDefaultAsync(w => w.WeeklyLogId == id);

                    if (WeeklyLog == null)
                    {
                        ErrorMessage = "Weekly Log not found";
                        return RedirectToPage("/Supervisor/Supervision/Index");
                    }

                    Supervisor = await _context.Supervisor
                        .Where(s => s.DateDeleted == null)
                        .FirstOrDefaultAsync(s => s.AssignedId == WeeklyLog.SupervisorId);
                    
                    if (WeeklyLog.CoSupervisorId != null)
                    {
                        CoSupervisor = await _context.Supervisor
                            .Where(s => s.DateDeleted == null)
                            .FirstOrDefaultAsync(s => s.AssignedId == WeeklyLog.CoSupervisorId);

                        if (WeeklyLog.SupervisorId != username && WeeklyLog.CoSupervisorId != username)
                        {
                            ErrorMessage = "This is not your weekly log!";
                            return RedirectToPage("/Supervisor/Supervision/Index");
                        }
                    }
                    else
                    {
                        if (WeeklyLog.SupervisorId != username)
                        {
                            ErrorMessage = "This is not your weekly log!";
                            return RedirectToPage("/Supervisor/Supervision/Index");
                        }
                    }

                    Student = await _context.Student
                       .Where(s => s.DateDeleted == null)
                       .FirstOrDefaultAsync(s => s.AssignedId == WeeklyLog.StudentId);
                    
                    Af = new ApprovalForm
                    {
                        Comment = WeeklyLog.Comment
                    };

                    CanApprove = false;

                    // only supervisor can approved
                    if (WeeklyLog.SupervisorId == username && (WeeklyLog.WeeklyLogStatus == "New" || WeeklyLog.WeeklyLogStatus == "Modified"))
                    {
                        CanApprove = true;
                    }

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

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var weeklyLog = await _context.WeeklyLog.Where(w => w.DateDeleted == null).Include(w => w.Project).FirstOrDefaultAsync(w => w.WeeklyLogId == id);
            
            if (weeklyLog == null)
            {
                ErrorMessage = "Weekly log not found";
                return RedirectToPage("/Supervisor/Supervision/Index");
            }

            if (weeklyLog.WeeklyLogStatus != "New" && weeklyLog.WeeklyLogStatus != "Modified")
            {
                ErrorMessage = "Weekly log approved or require modification. Action denied";
                return RedirectToPage("/Supervisor/Supervision/SupervisedStudent", weeklyLog.ProjectId);
            }

            var project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == weeklyLog.ProjectId);

            // update weekly log
            weeklyLog.Comment = Af.Comment;
            weeklyLog.WeeklyLogStatus = "Approved";
            weeklyLog.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            var username = HttpContext.Session.GetString("_username");

            // get supervisor information
            var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == weeklyLog.SupervisorId);
            var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == weeklyLog.StudentId);

            var mailsubject = $"FYP System - Weekly Log {weeklyLog.WeeklyLogNumber} (Approved)";
            var mailbody = $@"Dear {student.StudentName}, 

The Weekly Log {weeklyLog.WeeklyLogNumber} for the project below had been approved: 

Project ID: {weeklyLog.Project.AssignedId}
Project Title: {weeklyLog.Project.ProjectTitle}

Meeting Date: {weeklyLog.WeeklyLogDate.ToShortDateString()}

Comments:
{Af.Comment}

Please navigate to the system menu under Weekly Log --> Weekly Log List to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{supervisor.SupervisorName}
(FYP Supervisor)";

            await _emailSender.SendEmailAsync(supervisor.SupervisorEmail, student.StudentEmail, mailsubject, mailbody);

            SuccessMessage = $"Weekly log approved.";

            return RedirectToPage("/Supervisor/Supervision/Index");
        }

        public async Task<IActionResult> OnPostModifyAsync(int id)
        {
            if (Af.Comment == null)
            {
                ErrorMessage = "Comment cannot be empty.";
                return RedirectToPage("/Supervisor/Supervision/ViewWeeklyLog", id);
            }

            var weeklyLog = await _context.WeeklyLog.Where(w => w.DateDeleted == null).Include(w => w.Project).FirstOrDefaultAsync(w => w.WeeklyLogId == id);

            if (weeklyLog == null)
            {
                ErrorMessage = "Weekly log not found";
                return RedirectToPage("/Supervisor/Supervision/Index");
            }

            if (weeklyLog.WeeklyLogStatus != "New" && weeklyLog.WeeklyLogStatus != "Modified")
            {
                ErrorMessage = "Modification not required. Action denied";
                return RedirectToPage("/Supervisor/Supervision/SupervisedStudent", weeklyLog.ProjectId);
            }

            var project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == weeklyLog.ProjectId);

            Console.WriteLine("ID: " + project.ProjectId);

            // update weekly log
            weeklyLog.Comment = Af.Comment;
            weeklyLog.WeeklyLogStatus = "Require Modification";
            weeklyLog.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            var username = HttpContext.Session.GetString("_username");

            // get supervisor information
            var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == weeklyLog.SupervisorId);
            var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == weeklyLog.StudentId);

            var mailsubject = $"FYP System - Weekly Log {weeklyLog.WeeklyLogNumber} (Require Modification)";
            var mailbody = $@"Dear {student.StudentName}, 

The Weekly Log {weeklyLog.WeeklyLogNumber} for the project below require some modification: 

Project ID: {weeklyLog.Project.AssignedId}
Project Title: {weeklyLog.Project.ProjectTitle}

Meeting Date: {weeklyLog.WeeklyLogDate.ToShortDateString()}

Comments:
{Af.Comment}

Please navigate to the system menu under Weekly Log --> Weekly Log List to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{supervisor.SupervisorName}
(FYP Supervisor)";

            await _emailSender.SendEmailAsync(supervisor.SupervisorEmail, student.StudentEmail, mailsubject, mailbody);

            ErrorMessage = $"Weekly log require modification.";

            return RedirectToPage("/Supervisor/Supervision/Index");
        }
    }
}