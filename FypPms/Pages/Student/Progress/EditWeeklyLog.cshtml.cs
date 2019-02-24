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

namespace FypPms.Pages.Student.Progress
{
    public class EditWeeklyLogModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<EditWeeklyLogModel> _logger;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public WeeklyLog WeeklyLog { get; set; }
        [BindProperty]
        public Models.Student Student { get; set; }
        [BindProperty]
        public Models.Project Project { get; set; }
        [BindProperty]
        public Models.Supervisor Supervisor { get; set; }
        [BindProperty]
        public Models.Supervisor CoSupervisor { get; set; }
        [BindProperty]
        public EditForm Ef { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public EditWeeklyLogModel(FypPmsContext context, ILogger<EditWeeklyLogModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        public class EditForm
        {
            [DisplayName("Meeting Date")]
            public DateTime Date { get; set; }
            [DisplayName("Work Done")]
            public string WorkDone { get; set; }
            [DisplayName("Work To Be Done")]
            public string WorkToBeDone { get; set; }
            [DisplayName("Problems Faced")]
            public string Problem { get; set; }
            [DisplayName("Comment by Supervisor")]
            public string Comment { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Student");

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
                        return RedirectToPage($"/{usertype}/Progress/Index");
                    }

                    Student = await _context.Student
                        .Where(s => s.DateDeleted == null)
                        .Include(s => s.Batch)
                        .Include(s => s.Project)
                        .FirstOrDefaultAsync(s => s.AssignedId == username);

                    if (WeeklyLog.StudentId != Student.AssignedId)
                    {
                        ErrorMessage = "Access Denied! This is not your weekly log!";
                        return RedirectToPage($"/{usertype}/Progress/Index");
                    }

                    if (WeeklyLog.WeeklyLogStatus != "Require Modification")
                    {
                        ErrorMessage = "Modification not required. Action denied";
                        return RedirectToPage("/Student/Progress/Index");
                    }

                    Project = await _context.Project
                        .Where(p => p.DateDeleted == null)
                        .FirstOrDefaultAsync(p => p.ProjectId == Student.ProjectId);

                    Supervisor = await _context.Supervisor
                        .Where(s => s.DateDeleted == null)
                        .FirstOrDefaultAsync(s => s.AssignedId == Project.SupervisorId);

                    if (Project.CoSupervisorId != null)
                    {
                        CoSupervisor = await _context.Supervisor
                            .Where(s => s.DateDeleted == null)
                            .FirstOrDefaultAsync(s => s.AssignedId == Project.CoSupervisorId);
                    }

                    Ef = new EditForm
                    {
                        Date = WeeklyLog.WeeklyLogDate,
                        WorkDone = WeeklyLog.WorkDone,
                        WorkToBeDone = WeeklyLog.WorkToBeDone,
                        Problem = WeeklyLog.Problem,
                        Comment = WeeklyLog.Comment
                    };

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
            if (Ef.WorkDone == null || Ef.WorkToBeDone == null || Ef.Problem == null || Ef.Date == null)
            {
                ErrorMessage = "The field cannot be empty.";
                return RedirectToPage("/Student/Progress/EditWeeklyLog", id);
            }

            var weeklyLog = await _context.WeeklyLog.Where(w => w.DateDeleted == null).Include(w => w.Project).FirstOrDefaultAsync(w => w.WeeklyLogId == id);

            if (weeklyLog == null)
            {
                ErrorMessage = "Weekly log not found";
                return RedirectToPage("/Student/Progress/Index");
            }

            if (weeklyLog.WeeklyLogStatus != "Require Modification")
            {
                ErrorMessage = "Modification not required. Action denied";
                return RedirectToPage("/Student/Progress/Index");
            }

            // update weekly log
            weeklyLog.WeeklyLogDate = Ef.Date;
            weeklyLog.WorkDone = Ef.WorkDone;
            weeklyLog.WorkToBeDone = Ef.WorkToBeDone;
            weeklyLog.Problem = Ef.Problem;
            weeklyLog.WeeklyLogStatus = "Modified";
            weeklyLog.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            var username = HttpContext.Session.GetString("_username");
            
            // get supervisor information
            var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == weeklyLog.SupervisorId);
            var cosupervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == weeklyLog.CoSupervisorId);
            var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == weeklyLog.StudentId);

            var mailsubject = $"FYP System - Weekly Log {weeklyLog.WeeklyLogNumber} (Modified)";
            var mailbody = $@"Dear {supervisor.SupervisorName}, 

The Weekly Log {weeklyLog.WeeklyLogNumber} for the project below had been modified: 

Project ID: {weeklyLog.Project.AssignedId}
Project Title: {weeklyLog.Project.ProjectTitle}

Meeting Date: {Ef.Date.ToShortDateString()}

Comments from Supervisor:
{weeklyLog.Comment}

Work Done:
{Ef.WorkDone}

Work to Be Done:
{Ef.WorkToBeDone}

Problems Faced:
{Ef.Problem}

Please navigate to the system menu under My Supervision --> Supervision List to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{student.StudentName}
{student.AssignedId}
(FYP Student)";

            await _emailSender.SendEmailAsync(student.StudentEmail, supervisor.SupervisorEmail, mailsubject, mailbody);

            if(cosupervisor != null)
            {
                var mailsubject2 = $"FYP System - Weekly Log {weeklyLog.WeeklyLogNumber} (Modified)";
                var mailbody2 = $@"Dear {cosupervisor.SupervisorName}, 

The Weekly Log {weeklyLog.WeeklyLogNumber} for the project below had been modified: 

Project ID: {weeklyLog.Project.AssignedId}
Project Title: {weeklyLog.Project.ProjectTitle}

Meeting Date: {Ef.Date.ToShortDateString()}

Comments from Supervisor:
{weeklyLog.Comment}

Work Done:
{Ef.WorkDone}

Work to Be Done:
{Ef.WorkToBeDone}

Problems Faced:
{Ef.Problem}

Please navigate to the system menu under My Supervision --> Supervision List to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{student.StudentName}
{student.AssignedId}
(FYP Student)";

                await _emailSender.SendEmailAsync(student.StudentEmail, cosupervisor.SupervisorEmail, mailsubject2, mailbody2);
            }

            SuccessMessage = $"Weekly log modified and sent successfully.";

            return RedirectToPage("/Student/Progress/Index");
        }
    }
}